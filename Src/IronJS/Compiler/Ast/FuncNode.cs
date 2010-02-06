/* ****************************************************************************
 *
 * Copyright (c) Fredrik Holmström
 *
 * This source code is subject to terms and conditions of the Microsoft Public License. A 
 * copy of the license can be found in the License.html file at the root of this distribution. If 
 * you cannot locate the  Microsoft Public License, please send an email to 
 * dlr@microsoft.com. By using this source code in any fashion, you are agreeing to be bound 
 * by the terms of the Microsoft Public License.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************/
using System;
using System.Collections.Generic;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;
using IronJS.Tools;
using Microsoft.Scripting.Utils;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast {

    #region Aliases
    using Et = Expression;
    using EtParam = ParameterExpression;
    using ParamTuple = Tuple<ParameterExpression, ParameterExpression>;
    #endregion

    public class FuncNode : Node {
        public FuncNode Parent { get; set; }
        public IdentifierNode Name { get; set; }
        public INode Body { get; set; }
        public HashSet<INode> Returns { get; set; }

        public Dictionary<string, IjsClosureVar> ClosesOver { get; set; }
        public Dictionary<string, IjsParameter> Parameters { get; set; }
        public Dictionary<string, IjsLocalVar> Locals { get; set; }
        public Dictionary<string, IjsLocalVar> Globals { get; set; }

        public bool IsNamed { get { return !IsLambda; } }
        public bool IsLambda { get { return Name == null; } }
        public bool IsBranched { get; set; }
        public bool IsSimple { get { return !IsBranched; } }
        public bool IsGlobalScope { get { return Locals == Globals; } }
        public bool IsNotGlobalScope { get { return !IsGlobalScope; } }

        public override Type ExprType { get { return IjsTypes.Object; } }
        public Type ReturnType { get { return IjsTypes.Dynamic; } }

        /*
         * Compilation properties
         */
        public LabelTarget ReturnLabel { get; protected set; }
        public EtParam ClosureParm { get; protected set; }
        public MemberExpression GlobalField { get; protected set; }
        public MemberExpression ContextField { get; protected set; }
        public Dictionary<Type, EtParam> CallProxies { get; set; }

        public FuncNode(IdentifierNode name, IEnumerable<string> parameters, INode body, ITree node)
            : base(NodeType.Func, node) {
            Body = body;
            Name = name;

            Locals = new Dictionary<string, IjsLocalVar>();
            Parameters = new Dictionary<string, IjsParameter>();
            ClosesOver = new Dictionary<string, IjsClosureVar>();

            if (parameters != null)
                foreach (string param in parameters)
                    Parameters.Add(param, new IjsParameter());

            Returns = new HashSet<INode>();

            if (IsNamed)
                Name.IsDefinition = true;
        }


        public override INode Analyze(FuncNode func) {
            if (func != this) {
                Parent = func;
                Globals = func.Globals;

                if (IsNamed) {
                    Name.Analyze(func);
                    (Name.VarInfo as IjsLocalVar).AssignedFrom.Add(this);
                    //HACK: We just assume that VarInfo is a IjsLocalVar here
                }
            }

            Body = Body.Analyze(this);

            return this;
        }

        public override Expression Compile(FuncNode func) {
            return AstTools.New(
                typeof(IjsFunc),
                Et.Constant(this),
                AstTools.New(typeof(IjsClosure), func.ContextField, func.GlobalField)
            );
        }

        public TFunc Compile<TFunc, TGuard>(Type[] realTypes, out TGuard guard) {
            Type[] types = typeof(TFunc).GetGenericArguments();
            Type[] paramTypes = ArrayTools.DropFirstAndLast(types);

            ClosureParm = Et.Parameter(typeof(IjsClosure), "__closure__");
            GlobalField = Et.Field(ClosureParm, "Globals");
            ReturnLabel = Et.Label(ReturnType, "__return__");
            CallProxies = new Dictionary<Type, ParameterExpression>();
            ContextField = Et.Field(ClosureParm, "Context");

            EtParam[] defaults;
            EtParam[] boxedParams;

            ParamTuple[] paramPairs = SetParameterTypes(realTypes, paramTypes, out defaults, out boxedParams);
            ParamTuple[] oddPairs = ArrayTools.Filter(paramPairs, delegate(ParamTuple x) {
                return x.Item1 != x.Item2;
            });

            Et bodyExpr = Body.Compile(this);

            Expression<TFunc> lambda = Et.Lambda<TFunc>(
                Et.Block(
                    // Concat all parameters used
                    ArrayTools.Concat(
                        ArrayTools.Map(oddPairs, delegate(ParamTuple x) { return x.Item1; }),
                        ArrayTools.Concat(DictionaryTools.GetValues(CallProxies), GetLocalsExprs())
                    ).Concat(defaults),

                    // Assigns all call proxies used inside this function
                    AstTools.BuildBlock(CallProxies, delegate(KeyValuePair<Type, ParameterExpression> pair) { return Et.Assign(pair.Value, AstTools.New(pair.Key)); }),

                    // Set up boxes for parameters that are closures
                    AstTools.BuildBlock(boxedParams, delegate(EtParam param) { return Et.Assign(param, AstTools.New(param.Type)); }),

                    // Assigns parameter values to correctly typed local variables (casting as needed)
                    AstTools.BuildBlock(oddPairs, delegate(ParamTuple pair) { return AstTools.Assign(pair.Item1, pair.Item2); }),

                    // Sets all named parameters that didn't get values to Undefined
                    AstTools.BuildBlock(defaults, delegate(EtParam expr) { return AstTools.Assign(expr, Undefined.Expr); }),

                    // Sets up all local variables that are closures
                    AstTools.BuildBlock(
                        ArrayTools.Filter(DictionaryTools.GetValues(Locals), delegate(IjsLocalVar variable) { return variable.IsClosedOver; }),
                        delegate(IjsLocalVar variable) { return Et.Assign(variable.Expr, AstTools.New(variable.Expr.Type)); }
                    ),

                    // This is the body of our function
                    bodyExpr,

                    // Return label target
                    Et.Label(ReturnLabel, Undefined.Expr)
                ),

                // Set up parameters
                new[] { ClosureParm }.Concat(IEnumerableTools.Map(paramPairs, delegate(ParamTuple pair) { return pair.Item2; }))
            );

            // Build guard
            guard = Et.Lambda<TGuard>(
                BuildTypeCheck(oddPairs),

                // Sadly we have to send all parameters to the Guard delegate
                IEnumerableTools.Map(paramPairs, delegate(ParamTuple pair) { return pair.Item2; })
            ).Compile();

            // Reset all parameters to Undefined
            ResetParameterTypes();

            return lambda.Compile();
        }

        ParamTuple[] SetParameterTypes(Type[] realTypes, Type[] paramTypes, out EtParam[] defaults, out EtParam[] boxedParams) {

            List<EtParam> boxedList = new List<EtParam>();
            List<EtParam> defaultsList = new List<EtParam>();
            ParamTuple[] paramPairs = new ParamTuple[realTypes.Length];

            int index = 0;
            foreach (KeyValuePair<string, IjsParameter> param in Parameters) {
                if (index < realTypes.Length) {

                    if (param.Value.IsClosedOver) {
                        EtParam box = Et.Parameter(TypeTools.StrongBoxType.MakeGenericType(realTypes[index]), "__box:" + param.Key + "__");
                        paramPairs[index] = Tuple.Create(box, Et.Parameter(paramTypes[index], "__param:" + param.Key + "__"));
                        boxedList.Add(box);

                    } else {
                        paramPairs[index] = CreateParamTuple(realTypes[index], paramTypes[index], param.Key);
                    }

                    param.Value.Expr = paramPairs[index].Item1;

                } else {

                    if (param.Value.IsClosedOver) {
                        param.Value.Expr = Et.Parameter(TypeTools.StrongBoxType.MakeGenericType(IjsTypes.Dynamic), "__box:" + param.Key + "__");
                        boxedList.Add(param.Value.Expr);
                    } else {
                        param.Value.Expr = Et.Parameter(IjsTypes.Dynamic, "__" + param.Key + "__");
                    }

                    defaultsList.Add(param.Value.Expr);
                }

                ++index;
            }

            for (; index < realTypes.Length; ++index) {
                paramPairs[index] = CreateParamTuple(realTypes[index], paramTypes[index], "arg:" + index);
            }

            defaults = defaultsList.ToArray();
            boxedParams = boxedList.ToArray();

            return paramPairs;
        }

        ParamTuple CreateParamTuple(Type real, Type param, string name) {

            if (real == param) {
                EtParam etparm = Et.Parameter(param, "__" + name + "__");
                return Tuple.Create(etparm, etparm);

            } else {

                return Tuple.Create(
                    Et.Parameter(real, "__real:" + name + "__"),
                    Et.Parameter(param, "__param:" + name + "__")
                );
            }

        }

        public override void Print(StringBuilder writer, int indent) {
            string indentStr = new String(' ', indent * 2);
            string indentStr2 = new String(' ', (indent + 1) * 2);
            string indentStr3 = new String(' ', (indent + 2) * 2);

            writer.AppendLine(indentStr
                + "(" + NodeType
                + (" " + Name + " ").TrimEnd()
                + " " + TypeTools.ShortName(ReturnType)
            );

            if (ClosesOver.Count > 0) {
                writer.AppendLine(indentStr2 + "(Closure");

                foreach (KeyValuePair<string, IjsClosureVar> kvp in ClosesOver)
                    writer.AppendLine(indentStr3 + "(" + kvp.Key + " " + TypeTools.ShortName(ExprType) + ")");

                writer.AppendLine(indentStr2 + ")");
            }

            if (Parameters.Count > 0) {
                writer.AppendLine(indentStr2 + "(Parameters");

                foreach (KeyValuePair<string, IjsParameter> kvp in Parameters)
                    writer.AppendLine(indentStr3 + "(" + kvp.Key + " " + TypeTools.ShortName(ExprType) + ")");

                writer.AppendLine(indentStr2 + ")");
            }

            Body.Print(writer, indent + 1);
            writer.AppendLine(indentStr + ")");
        }

        internal ParameterExpression GetCallProxy(Type type) {
            ParameterExpression expr;

            if (CallProxies.TryGetValue(type, out expr))
                return expr;

            return CallProxies[type] = Et.Parameter(type, "__callproxy__");
        }

        internal bool IsGlobal(IjsIVar varInfo) {
            if (varInfo is IjsLocalVar)
                return Globals.ContainsValue(varInfo as IjsLocalVar);

            return false;
        }

        internal bool IsLocal(IjsIVar varInfo) {
            if (varInfo is IjsLocalVar)
                return Locals.ContainsValue(varInfo as IjsLocalVar);

            if (varInfo is IjsParameter)
                return Parameters.ContainsValue(varInfo as IjsParameter);

            return false;
        }

        internal bool IsClosedOver(IjsIVar varInfo) {
            if (varInfo is IjsClosureVar)
                return ClosesOver.ContainsValue(varInfo as IjsClosureVar);

            return false;
        }

        internal bool IsLocal(string name) {
            return Locals.ContainsKey(name);
        }

        internal bool IsParameter(string name) {
            return Parameters.ContainsKey(name);
        }

        internal IjsCloseableVar CreateLocal(string name) {
            return Locals[name] = new IjsLocalVar();
        }

        internal IjsCloseableVar GetLocal(string name) {
            if (IsLocal(name))
                return Locals[name];

            if (IsParameter(name))
                return Parameters[name];

            throw new AstCompilerError("No local variable named '{0}' exists", name);
        }

        internal IjsIVar GetNonLocal(string name) {
            if (Parent == null)
                return CreateLocal(name);

            FuncNode parent = Parent;
            HashSet<FuncNode> parentFunctions = new HashSet<FuncNode>();

            while (true) {
                if (parent.IsLocal(name) || parent.IsParameter(name)) {
                    if (parent.IsGlobalScope) {
                        return parent.GetLocal(name);
                    } else {
                        IjsCloseableVar varInfo = parent.GetLocal(name);
                        varInfo.IsClosedOver = true;

                        foreach (FuncNode subParent in parentFunctions)
                            if (!subParent.ClosesOver.ContainsKey(name))
                                subParent.ClosesOver.Add(name, new IjsClosureVar(name, subParent));

                        ClosesOver.Add(name, new IjsClosureVar(name, this));
                        return ClosesOver[name];
                    }
                }

                if (parent.IsGlobalScope)
                    break;

                parentFunctions.Add(parent);
                parent = parent.Parent;
            }

            // parent = global scope
            return parent.CreateLocal(name);
        }

        Et BuildTypeCheck(ParamTuple[] oddPairs) {
            if (oddPairs.Length == 0)
                return AstTools.Constant(true);

            Type type;

            if (AstTools.IsStrongBox(oddPairs[0].Item1)) {
                type = oddPairs[0].Item1.Type.GetGenericArguments()[0];
            } else {
                type = oddPairs[0].Item1.Type;
            }

            return Et.AndAlso(
                Et.TypeIs(oddPairs[0].Item2, type),
                BuildTypeCheck(
                    ArrayUtils.RemoveFirst(oddPairs)
                )
            );
        }

        EtParam[] GetLocalsExprs() {
            if (Locals == Globals)
                return new EtParam[0];

            return IEnumerableTools.Map(
                Locals,
                delegate(KeyValuePair<string, IjsLocalVar> pair) {
                    return pair.Value.Expr;
                }
            );
        }

        void ResetParameterTypes() {
            foreach (KeyValuePair<string, IjsParameter> param in Parameters) {
                param.Value.Expr = null;
            }
        }
    }
}

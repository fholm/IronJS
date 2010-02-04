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

using Antlr.Runtime.Tree;
using Antlr.Runtime.Tree;
using IronJS.Compiler.Tools;
using IronJS.Runtime2.Binders;
using IronJS.Runtime2.Js;
using IronJS.Runtime2.Js;
using IronJS.Tools;
using IronJS.Tools;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;
using Microsoft.Scripting.Utils;
using System;
using System;
using System.Collections.Generic;
using System.Collections.Generic;
using System.Text;



namespace IronJS.Compiler.Ast
{
    using ArrayUtils = Microsoft.Scripting.Utils.ArrayUtils;
	using AstUtils = Utils;
	using Et = Expression;
	using ParamTuple = Tuple<ParameterExpression, ParameterExpression>;

    public class FuncNode : Node
    {
        public FuncNode Parent { get; protected set; }
        public IdentifierNode Name { get; protected set; }
        public INode Body { get; protected set; }
        public HashSet<INode> Returns { get; protected set; }

        public Dictionary<string, IjsClosureVar> ClosesOver { get; protected set; }
        public Dictionary<string, IjsParameter> Parameters { get; protected set; }
        public Dictionary<string, IjsLocalVar> Locals { get; protected set; }
        public Dictionary<string, IjsLocalVar> Globals { get; protected set; }

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
        public ParameterExpression ClosureParm { get; protected set; }
        public MemberExpression GlobalField { get; protected set; }
        public Dictionary<Type, ParameterExpression> CallProxies { get; set; }

        public FuncNode(IdentifierNode name, IEnumerable<string> parameters, INode body, ITree node)
            : base(NodeType.Func, node)
        {
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


        public override INode Analyze(FuncNode func)
        {
            if (func != this)
            {
                Parent = func;
                Globals = func.Globals;

                if (IsNamed)
                {
                    Name.Analyze(func);
                    (Name.VarInfo as IjsLocalVar).AssignedFrom.Add(this);
                }
            }

            Body = Body.Analyze(this);

            return this;
        }

        public override Expression EtGen(FuncNode func)
        {
            return IjsEtGenUtils.New(
                typeof(IjsFunc),
                Et.Constant(this),
                IjsEtGenUtils.New(
                    typeof(IjsClosure),
                    func.GlobalField
                )
            );
        }

        public ParameterExpression GetCallProxy(Type type)
        {
            ParameterExpression expr;

            if (CallProxies.TryGetValue(type, out expr))
                return expr;

            return CallProxies[type] = Et.Parameter(type, "__callproxy__");
        }

        public TFunc Compile<TFunc, TGuard>(Type[] inTypes, out TGuard guard)
        {
            Type[] types = typeof(TFunc).GetGenericArguments();
            Type closureType = types[0];
            Type[] paramTypes = ArrayTools.DropFirstAndLast(types);

            ClosureParm = Et.Parameter(closureType, "__closure__");
            GlobalField = Et.Field(ClosureParm, "Globals");
            ReturnLabel = Et.Label(ReturnType, "__return__");
            CallProxies = new Dictionary<Type, ParameterExpression>();

            int n = 0;
            ParamTuple[] paramPairs = ArrayTools.Map(paramTypes, x => {
                ParameterExpression parm;
                ParameterExpression real;

                if (paramTypes[n] == inTypes[n])
                {
                    parm = real = Et.Parameter(paramTypes[n], "__arg" + n + "__");
                }
                else
                {
                    parm = Et.Parameter(inTypes[n], "__arg" + n + "__");
                    real = Et.Parameter(paramTypes[n], "__arg " + n + "__");
                }

                ++n;

                return Tuple.Create(parm, real);
            }
            );

            n = 0;
            foreach (KeyValuePair<string, IjsParameter> param in Parameters)
                param.Value.Expr = paramPairs[n++].Item2;

            ParamTuple[] oddPairs = 
            ArrayTools.Filter(paramPairs, delegate(ParamTuple x) {
                return x.Item1 != x.Item2;
            });

            Et body = Body.EtGen(this);

            Expression<TFunc> lambda = Et.Lambda<TFunc>(
                Et.Block(
                    ArrayTools.Concat(
                        ArrayTools.Map(
                            oddPairs,
                            delegate(ParamTuple x) {
                                return x.Item2;
                            }
                        ),
                        DictionaryTools.GetValues(CallProxies)
                    ),

                    AstTools.BuildBlock(CallProxies, delegate(KeyValuePair<Type, ParameterExpression> pair){
                        return Et.Assign(pair.Value, IjsEtGenUtils.New(pair.Key));
                    }),

                    AstTools.BuildBlock(oddPairs, delegate(ParamTuple pair) {
                        return Et.Assign(pair.Item2, Et.Convert(pair.Item1, pair.Item2.Type));
                    }),

                    Et.Block(
                        (Locals != Globals) // HACK
                            ? IEnumerableTools.Map(Locals, delegate(KeyValuePair<string, IjsLocalVar> pair){ return pair.Value.Expr; })
                            : new ParameterExpression[] {},
                        body,
                        Et.Label(
                            ReturnLabel,
                            Et.Default(ReturnType)
                        )
                    )
                ),
                new[] { ClosureParm }.Concat(
                    IEnumerableTools.Map(paramPairs, delegate(ParamTuple pair) {
                        return pair.Item1;
                    })
                )
            );

            guard = Et.Lambda<TGuard>(
                BuildTypeCheck(oddPairs),
                IEnumerableTools.Map(paramPairs, delegate(ParamTuple pair){
                    return pair.Item1;
                })
            ).Compile();

            return lambda.Compile();
        }

        Et BuildTypeCheck(ParamTuple[] oddPairs)
        {
            if (oddPairs.Length == 0)
                return IjsEtGenUtils.Constant(true);

            return Et.AndAlso(
                Et.TypeIs(
                    oddPairs[0].Item1,
                    oddPairs[0].Item2.Type
                ),
                BuildTypeCheck(
                    Microsoft.Scripting.Utils.ArrayUtils.RemoveFirst(oddPairs)
                )
            );
        }

        internal bool IsGlobal(IjsIVar varInfo)
        {
            if(varInfo is IjsLocalVar)
                return Globals.ContainsValue(varInfo as IjsLocalVar);

            return false;
        }

        internal bool IsLocal(IjsIVar varInfo)
        {
            if (varInfo is IjsLocalVar)
                return Locals.ContainsValue(varInfo as IjsLocalVar);

            if (varInfo is IjsParameter)
                return Parameters.ContainsValue(varInfo as IjsParameter);

            return false;
        }

        internal bool IsClosedOver(IjsIVar varInfo)
        {
            if (varInfo is IjsClosureVar)
                return ClosesOver.ContainsValue(varInfo as IjsClosureVar);

            return false;
        }

        public bool HasLocal(string name)
        {
            return Locals.ContainsKey(name);
        }

        public bool HasParameter(string name)
        {
            return Parameters.ContainsKey(name);
        }

        public IjsCloseableVar CreateLocal(string name)
        {
            return Locals[name] = new IjsLocalVar();
        }

        public IjsCloseableVar GetLocal(string name)
        {
            if (HasLocal(name))
                return Locals[name];

            if (HasParameter(name))
                return Parameters[name];

            throw new AstCompilerError("No local variable named '{0}' exists", name);
        }

        public IjsIVar GetNonLocal(string name)
        {
            if (Parent == null)
                return CreateLocal(name);

            FuncNode parent = Parent;
            HashSet<FuncNode> parentFunctions = new HashSet<FuncNode>();

            while (true)
            {
                if (parent.HasLocal(name) || parent.HasParameter(name))
                {
                    if (parent.IsGlobalScope)
                    {
                        return parent.GetLocal(name);
                    }
                    else
                    {
                        IjsCloseableVar varInfo = parent.GetLocal(name);
                        varInfo.IsClosedOver = true;

                        foreach (FuncNode subParent in parentFunctions)
                            if(!subParent.ClosesOver.ContainsKey(name))
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

        public override void Print(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);
            string indentStr2 = new String(' ', (indent + 1) * 2);
            string indentStr3 = new String(' ', (indent + 2) * 2);

            writer.AppendLine(indentStr 
                + "(" + NodeType 
                + (" " + Name + " ").TrimEnd() 
                + " " + TypeTools.ShortName(ReturnType)
            );

            if (ClosesOver.Count > 0)
            {
                writer.AppendLine(indentStr2 + "(Closure");

                foreach (KeyValuePair<string, IjsClosureVar> kvp in ClosesOver)
                    writer.AppendLine(indentStr3 + "(" + kvp.Key + " " + TypeTools.ShortName(ExprType) + ")");

                writer.AppendLine(indentStr2 + ")");
            }

            if(Parameters.Count > 0)
            {
                writer.AppendLine(indentStr2 + "(Parameters");

                foreach (KeyValuePair<string, IjsParameter> kvp in Parameters)
                    writer.AppendLine(indentStr3 + "(" + kvp.Key + " " + TypeTools.ShortName(ExprType) + ")");

                writer.AppendLine(indentStr2 + ")");
            }

            Body.Print(writer, indent + 1);
            writer.AppendLine(indentStr + ")");
        }
    }
}

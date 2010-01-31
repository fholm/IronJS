/* ****************************************************************************
 *
 * Copyright (c) Microsoft Corporation. 
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
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Threading;
using IronJS.Compiler.Ast;
using IronJS.Compiler.Optimizer;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Microsoft.Scripting.Generation;
using Et = System.Linq.Expressions.Expression;
using EtParam = System.Linq.Expressions.ParameterExpression;

namespace IronJS.Compiler
{
    public class IjsEtGenerator
    {
        public AssemblyGen AsmGen { get; protected set; }
        public ModuleBuilder ModGen { get; protected set; }
        public TypeGen TypeGen { get { return TypeGenStack.Peek(); } }
        public Stack<TypeGen> TypeGenStack { get; protected set; }
        public EtParam GlobalsExpr { get; protected set; }
        public EtParam ClosureExpr { get; protected set; }
        public IjsScope Scope { get { return Scopes.Peek(); } }
        public Stack<IjsScope> Scopes { get; protected set; }
        public List<Tuple<int, MemberExpression, Ast.FuncNode>> Functions { get; protected set; }

        string _assemblyNameFormat = "_ijs_asm{0}";
        string _callMethodName = "_call";
        string _callSitesClassName = "_callSites";
        string _callSiteFieldFormat = "_{0}";
        string _methodInfoClassName = "_methods";
        string _funcClassNameFormat = "_func{0}";
        string _globalClassName = "_global";
        string _closureClassNameFormat = "_closure{0}";

        int _callSiteN = 0;
        TypeGen _callSiteType;
        public MemberExpression CreateCallSiteField<T>() where T : class
        {
            return Et.Field(
                null,
                _callSiteType.AddStaticField(
                    typeof(CallSite<T>), 
                    String.Format(_callSiteFieldFormat, _callSiteN++)
                )
            );
        }

        TypeGen _methodInfoType;
        Dictionary<string, Et> _methodInfoCache
            = new Dictionary<string, Et>();
        public Et CreateMethodInfoField(string className)
        {
            Et expr;

            if(!_methodInfoCache.TryGetValue(className, out expr))
            {
                var getType = typeof(Type).GetMethod("GetType", new[] { typeof(string) });
                var getMethod = typeof(Type).GetMethod("GetMethod", new[] { typeof(string) });

                expr = Et.Field(
                    null,
                    _methodInfoType.AddStaticField(
                        typeof(MethodInfo),
                        className
                    )
                );

                _methodInfoCache[className] = expr;
            }

            return expr;
        }

        static int _asmN = 0;
        public MethodInfo Generate(List<Ast.INode> astNodes, IjsContext context)
        {
            // Setup common properties
            Functions = new List<Tuple<int, MemberExpression, Ast.FuncNode>>();
            TypeGenStack = new Stack<TypeGen>();
            Scopes = new Stack<IjsScope>();

            // Setup type generator
            AsmGen = new AssemblyGen(new AssemblyName(String.Format(_assemblyNameFormat, Interlocked.Increment(ref _asmN))), ".\\", ".dll", false);
            ModGen = AsmGen.AssemblyBuilder.GetDynamicModule(AsmGen.AssemblyBuilder.GetName().Name);
            _callSiteType = new TypeGen(AsmGen, ModGen.DefineType(_callSitesClassName));
            _methodInfoType = new TypeGen(AsmGen, ModGen.DefineType(_methodInfoClassName));

            // Expression used by all methods
            GlobalsExpr = Et.Parameter(typeof(IjsObj), "$globals");
            
            var miGlobal = CompileGlobal(astNodes);

            _methodInfoType.FinishType();
            _callSiteType.FinishType();
            AsmGen.SaveAssembly();

            return miGlobal;
        }

        int _funN;
        internal MethodInfo CompileFunction(IEnumerable<Ast.IdentifierNode> parameters, Ast.INode body, IjsFuncInfo funcInfo)
        {
            Scopes.Push(new IjsScope(funcInfo));

            var closureTypeGen = new TypeGen(AsmGen, ModGen.DefineType(string.Format(_closureClassNameFormat, _funN)));

            foreach (var enclosedId in funcInfo.ClosesOver)
                closureTypeGen.TypeBuilder.DefineField(
                    enclosedId.Name,
                    typeof(IjsClosureCell<>).MakeGenericType(enclosedId.ExprType), 
                    FieldAttributes.Public
                );

            var closureType = closureTypeGen.FinishType();
            ClosureExpr = Et.Parameter(closureType, "$closure");

            TypeGenStack.Push(new TypeGen(AsmGen, ModGen.DefineType(string.Format(_funcClassNameFormat, _funN++))));

            var paramsList = new List<EtParam>();

            foreach (var parameter in parameters)
                DefineVar(parameter.VarInfo);

            paramsList.Add(GlobalsExpr);

            if (funcInfo.ClosesOver.Count > 0)
            {
                funcInfo.ClosureType = closureType;
                paramsList.Add(ClosureExpr);
            }

            CompileFunction(
                paramsList.Concat(
                    Scope.Variables
                        .Where(x => x.Value.Item2.IsParameter)
                        .Select(x => x.Value.Item1)
                ),
                Et.Block(
                    body.EtGen(this),
                    Et.Label(
                        Scope.ReturnLabel,
                        Undefined.StaticExpr
                    )
                ),
                funcInfo.ReturnType
            );

            Scopes.Pop();

            return TypeGenStack.Pop().FinishType().GetMethod(_callMethodName);
        }

        MethodInfo CompileGlobal(List<Ast.INode> astNodes)
        {
            var getType = typeof(Type).GetMethod("GetType", new[] { typeof(string) });
            var getMethod = typeof(Type).GetMethod("GetMethod", new[] { typeof(string) });
            var globalExprs = new Ast.BlockNode(astNodes, null).EtGen(this);

            TypeGenStack.Push(new TypeGen(AsmGen, ModGen.DefineType(_globalClassName)));

            CompileFunction(
                new[] { GlobalsExpr },
                Et.Block(
                    Et.Block(
                        _methodInfoCache.Select(x =>
                            Et.Assign(
                                x.Value,
                                Et.Call(
                                    Et.Call(
                                        getType,
                                        Constant(x.Key)
                                    ),
                                    getMethod,
                                    Constant(_callMethodName)
                                )
                            )
                        )
                    ),
                    Et.Block(globalExprs)
                ),
                typeof(object)
            );

            return TypeGenStack.Pop().FinishType().GetMethod(_callMethodName);
        }

        void CompileFunction(IEnumerable<EtParam> parameters, Et body, Type returnType)
        {
            var mb = TypeGen.TypeBuilder.DefineMethod(
                _callMethodName,
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.Final, 
                CallingConventions.Standard,
                returnType, 
                parameters.Select(x => x.Type).ToArray()
            );

            IEnumerable<EtParam> variables = new EtParam[0];

            if (Scopes.Count > 0)
            {
                variables =
                    Scope.Variables
                        .Where(x => !x.Value.Item2.IsParameter)
                        .Select(x => x.Value.Item1);
            }

            Et.Lambda(
                Et.Block(variables, body),
                parameters
            ).CompileToMethod(mb);
        }

        internal Et Constant<T>(T value)
        {
            return Et.Constant(value);
        }

        internal Tuple<EtParam, IjsVarInfo> DefineVar(IjsVarInfo varInfo)
        {
            return Scope[varInfo.Name] = Tuple.Create(
                Et.Variable(varInfo.ExprType, varInfo.Name), varInfo
            );
        }

        internal Et GenAssignEt(Ast.INode target, Ast.INode Value)
        {
            return GenAssignEt(target, Value.EtGen(this));
        }

        private Et GenAssignEt(Ast.INode target, Et value)
        {
            // foo = <expr>
            var idNode = target as IdentifierNode;
            if (idNode != null)
            {
                if (idNode.IsGlobal)
                {
                    return Et.Call(
                        GlobalsExpr,
                        typeof(IjsObj).GetMethod("Set"),
                        Constant<string>(idNode.Name),
                        EtUtils.Box2(value)
                    );
                }
                else
                {
                    if (idNode.VarInfo.IsClosedOver)
                    {
                        var typ = typeof(IjsClosureCell<>).MakeGenericType(idNode.VarInfo.ExprType);
                        var variable = idNode.IsDefinition ? DefineClosedVar(idNode.VarInfo) : Scope[idNode.Name];

                        if (idNode.IsDefinition)
                        {
                            Et[] ets = new Et[2];

                            ets[0] = Et.Assign(
                                variable.Item1,
                                Et.New(typ)
                            );

                            if (target.ExprType == value.Type)
                            {
                                ets[1] = Et.Assign(
                                    Et.Field(
                                        variable.Item1,
                                        typ.GetField("Value")
                                    ),
                                    value
                                );
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }


                            return Et.Block(ets);
                        }
                        else
                        {
                            throw new NotImplementedException();
                        }

                        return null;
                    }
                    else
                    {
                        var variable = idNode.IsDefinition ? DefineVar(idNode.VarInfo) : Scope[idNode.Name];

                        if (target.ExprType == value.Type)
                            return Et.Assign(
                                variable.Item1,
                                value
                            );

                        if (variable.Item1.Type != typeof(object))
                            throw new ArgumentException("Expression types did not mach, but variable.Item1.Type is not typeof(object)");

                        return Et.Assign(
                            variable.Item1,
                            value
                        );
                    }
                }
            }

            throw new NotImplementedException();
        }

        private Tuple<EtParam, IjsVarInfo> DefineClosedVar(IjsVarInfo varInfo)
        {
            return Scope[varInfo.Name] = Tuple.Create(
                Et.Variable(
                    typeof(IjsClosureCell<>).MakeGenericType(varInfo.ExprType),
                    varInfo.Name
                ), varInfo
            );
        }
    }
}

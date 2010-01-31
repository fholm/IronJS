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

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using Microsoft.Scripting.Generation;
using System;
using IronJS.Runtime.Js;
using System.Runtime.CompilerServices;
using IronJS.Compiler.Optimizer;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler
{
    using Et = Expression;
    using EtParm = ParameterExpression;

    public class IjsEtGenerator
    {
        public int MethodCount { get; internal set; }
        public AssemblyGen AsmGen { get; protected set; }
        public ModuleBuilder ModGen { get; protected set; }
        public TypeGen TypeGen { get { return TypeGenStack.Peek(); } }
        public Stack<TypeGen> TypeGenStack { get; protected set; }
        public EtParm GlobalsExpr { get; protected set; }
        public IjsScope Scope { get { return Scopes.Peek(); } }
        public Stack<IjsScope> Scopes { get; protected set; }
        public List<Tuple<int, MemberExpression, Ast.LambdaNode>> Functions { get; protected set; }

        int _callSiteN = 0;
        TypeGen _callSiteType;
        public MemberExpression CreateCallSiteField<T>() where T : class
        {
            return Et.Field(
                null,
                _callSiteType.AddStaticField(
                    typeof(CallSite<T>), 
                    "_" + (_callSiteN++)
                )
            );
        }

        int _asmN = 0;
        public MethodInfo Generate(List<Ast.INode> astNodes, IjsContext context)
        {
            // Setup common properties
            Functions = new List<Tuple<int, MemberExpression, Ast.LambdaNode>>();
            TypeGenStack = new Stack<TypeGen>();
            Scopes = new Stack<IjsScope>();

            // Setup type generator
            AsmGen = new AssemblyGen(new AssemblyName("isj_asm_" + (_asmN++)), ".\\", ".dll", false);
            ModGen = AsmGen.AssemblyBuilder.GetDynamicModule(AsmGen.AssemblyBuilder.GetName().Name);
            _callSiteType = new TypeGen(AsmGen, ModGen.DefineType("$callsites"));

            // Expression used by all methods
            GlobalsExpr = Et.Parameter(typeof(IjsObj), "$globals");
            
            var miGlobal = CompileFunction(
                "$fun_global",
                new EtParm[] { }, 
                new Ast.BlockNode(astNodes, null),
                typeof(object)
            );

            _callSiteType.FinishType();
            AsmGen.SaveAssembly();

            return miGlobal;
        }

        internal MethodInfo CompileFunction(int funN, IEnumerable<Ast.IdentifierNode> parameters, Ast.INode body, Type returnType)
        {
            var etParameters = new List<EtParm>();

            foreach (var parameter in parameters)
                DefineVar(parameter.Variable);

            var mb = CompileFunction(
                "$fun_" + funN,
                Scope.Variables.Where(x => x.Value.Item2.IsParameter).Select(x => x.Value.Item1),
                body,
                returnType
            );

            return mb;
        }

        MethodInfo CompileFunction(string functionName, IEnumerable<EtParm> parameters, Ast.INode body, Type returnType, bool newType = true)
        {
            Scopes.Push(new IjsScope());
            TypeGenStack.Push(new TypeGen(AsmGen, ModGen.DefineType(functionName)));
            var bodyExpr = body.GenerateStatic(this);

            parameters = new[] {
                GlobalsExpr
            }.Concat(parameters);

            var mb = TypeGen.TypeBuilder.DefineMethod(
                "call",
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.Final, 
                CallingConventions.Standard
                ,
                returnType, 
                parameters.Select(x => x.Type).ToArray()
            );

            var lambda = Et.Lambda(
                Et.Block(
                    Scopes.Count > 0
                        ? Scope.Variables.Where(x => !x.Value.Item2.IsParameter).Select(x => x.Value.Item1)
                        : new EtParm[] { },
                    bodyExpr
                ),
                parameters
            );

            lambda.CompileToMethod(mb);

            Scopes.Pop();

            return TypeGenStack.Pop().FinishType().GetMethod("call");
        }

        internal Et Constant<T>(T value)
        {
            return Et.Constant(value);
        }

        internal Tuple<EtParm, Variable> DefineVar(Variable varInfo)
        {
            return Scope[varInfo.Name] = Tuple.Create(
                Et.Variable(varInfo.ExprType, varInfo.Name), varInfo
            );
        }
    }
}

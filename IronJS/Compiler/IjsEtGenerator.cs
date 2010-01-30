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

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler
{
    using Et = Expression;
    using EtParm = ParameterExpression;
    using AstUtils = Microsoft.Scripting.Ast.Utils;

    public class IjsEtGenerator
    {
        public int MethodCount { get; internal set; }
        public AssemblyGen AsmGen { get; protected set; }
        public ModuleBuilder ModGen { get; protected set; }
        public TypeGen TypeGen { get { return TypeGenStack.Peek(); } }
        public Stack<TypeGen> TypeGenStack { get; protected set; }
        public EtParm GlobalsExpr { get; protected set; }
        public IjsScope Scope { get; protected set; }
        public List<Tuple<int, MemberExpression, Ast.LambdaNode>> Functions { get; protected set; }

        public Type Generate(List<Ast.INode> astNodes, IjsContext context)
        {
            // Setup common properties
            MethodCount = -1;
            Functions = new List<Tuple<int, MemberExpression, Ast.LambdaNode>>();
            TypeGenStack = new Stack<Microsoft.Scripting.Generation.TypeGen>();

            // Setup type generator
            AsmGen = new AssemblyGen(new AssemblyName("IjsAsm@codeUnit"), ".\\", ".dll", false);
            ModGen = AsmGen.AssemblyBuilder.GetDynamicModule(AsmGen.AssemblyBuilder.GetName().Name);

            // Expression used by all methods
            GlobalsExpr = Et.Parameter(typeof(IjsObj), "$globals");

            var mbGlobal = CompileFunction(
                "fun$global",
                new EtParm[] { }, 
                new Ast.BlockNode(astNodes, null),
                typeof(object)
            );

            var x = AsmGen.SaveAssembly();
            return mbGlobal;
        }

        internal Type CompileFunction(int funN, IEnumerable<Ast.IdentifierNode> parameters, Ast.INode body, Type returnType)
        {
            Scope = new IjsScope();

            var etParameters = new List<EtParm>();

            foreach (var parameter in parameters)
                Scope[parameter.Name] = 
                    Et.Parameter(parameter.ExprType, "$" + parameter.Name);

            var mb = CompileFunction(
                "fun$" + funN,
                Scope.Variables.Values,
                body,
                returnType
            );

            return mb;
        }

        Type CompileFunction(string functionName, IEnumerable<EtParm> parameters, Ast.INode body, Type returnType)
        {
            TypeGenStack.Push(new TypeGen(AsmGen, ModGen.DefineType(functionName)));
            return CompileFunction(functionName, parameters, body.GenerateStatic(this), returnType, false);
        }

        Type CompileFunction(string functionName, IEnumerable<EtParm> parameters, Et body, Type returnType, bool newType = true)
        {
            if(newType)
                TypeGenStack.Push(new TypeGen(AsmGen, ModGen.DefineType(functionName)));

            parameters = new[] {
                GlobalsExpr
            }.Concat(parameters);

            var mb = TypeGen.TypeBuilder.DefineMethod(
                "func",
                MethodAttributes.Public | MethodAttributes.Static | MethodAttributes.Final, 
                CallingConventions.Standard,
                returnType, 
                parameters.Select(x => x.Type).ToArray()
            );

            var lambda = Et.Lambda(
                body,
                parameters
            );

            lambda.CompileToMethod(mb);

            return TypeGenStack.Pop().FinishType();
        }

        internal Et Constant<T>(T value)
        {
            return Et.Constant(value);
        }
    }
}

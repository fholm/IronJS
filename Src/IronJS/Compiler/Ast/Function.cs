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
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;
using Microsoft.Scripting.Utils;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast {


	public class Function : Node {

		public INode Name { get { return Children[0]; } }
		public INode Body { get { return Children[1]; } }
		public string[] ParameterNames { get; private set; }
		public bool IsLambda { get { return Name == null; } }
		public Type ReturnType { get { return IjsTypes.Dynamic; } }
		public override Type Type { get { return IjsTypes.Object; } }

		public Function(INode name, List<string> parameters, INode body, ITree node)
			: base(NodeType.Func, node) {
			ContractUtils.RequiresNotNull(parameters, "parameters");
			_variables = new Dictionary<string, Variable>();

			Children = new INode[parameters.Count + 3];
			Children[0] = name;
			Children[Children.Length - 1] = body;

			ParameterNames = ArrayUtils.Insert("@closure", ArrayUtils.MakeArray(parameters));

			Var(ParameterNames[0], new Parameter(ParameterNames[0]));
			Var(ParameterNames[0]).ForceType(typeof(IjsClosure));
			Children[1] = Var(ParameterNames[0]);

			for (int i = 0; i < parameters.Count; ++i) {
				Var(parameters[i], new Parameter(parameters[i]));
				Children[i + 2] = Var(parameters[i]);
			}
		}

		public override INode Analyze(Stack<Function> stack) {
			stack.Push(this);
			base.Analyze(stack);
			stack.Pop();
			return this;
		}

		Dictionary<string, Variable> _variables;
		public Variable Var(string name) { return _variables[name]; }
		public bool Var(string name, out Variable var) { return _variables.TryGetValue(name, out var); }
		public void Var(string name, Variable var) { _variables[name] = var; }

		/*
		public override Expression Compile(Function func) {
			return AstTools.New(
				typeof(IjsFunc),
				AstTools.Constant(this),
				AstTools.New(
					typeof(IjsClosure)//,
					//func.Context,
					//func.Globals
				)
			);
		}
		*/
	}
}

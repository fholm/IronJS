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
using IronJS.Runtime.Jit;
using IronJS.Runtime.Js;
using IronJS.Tools;
using Microsoft.Scripting.Utils;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Nodes {
	public class Lambda : Base {
		public INode Name { get { return Children[0]; } }
		public string[] ParamNames { get; private set; }
		public int ParamsCount { get { return ParamNames.Length; } }
		public INode Body { get { return Children[1]; } }
		public bool IsLambda { get { return Name == null; } }
		public Type ReturnType { get { return Types.Dynamic; } }
		public override Type Type { get { return Types.Object; } }

		public Lambda(INode name, List<string> parameters, INode body, ITree node)
			: base(NodeType.Func, node) {
			_variables = new Dictionary<string, Variable>();

			Children = new INode[parameters.Count + 3];
			Children[0] = name;
			Children[Children.Length - 1] = body;

			ParamNames = ArrayUtils.Insert("~closure", ArrayUtils.MakeArray(parameters));

			CreateVar(ParamNames[0], new Parameter(ParamNames[0]));
			Children[1] = Var(ParamNames[0]);

			for (int i = 0; i < parameters.Count; ++i) {
				CreateVar(parameters[i], new Parameter(parameters[i]));
				Children[i + 2] = Var(parameters[i]);
			}
		}

		public override INode Analyze(Stack<Lambda> stack) {
			stack.Push(this);
			base.Analyze(stack);
			stack.Pop();
			return this;
		}

		Dictionary<string, Variable> _variables;
		public Variable Var(string name) { return _variables[name]; }
		public bool Var(string name, out Variable var) { return _variables.TryGetValue(name, out var); }
		public void CreateVar(string name, Variable var) {
			if (_variables.ContainsKey(name))
				throw new AstError("A variable named '" + name + "' already exist");

			_variables[name] = var;
		}

		public override Expression Compile(JitContext func) {
			return AstTools.New(
				typeof(Function),
				AstTools.Constant(this),
				AstTools.New(
					typeof(Closure)//,
					//func.Context,
					//func.Globals
				)
			);
		}
	}
}

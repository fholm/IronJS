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
using IronJS.Runtime.Js;
using IronJS.Tools;
using IronJS.Ast.Tools;
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
		public INode Body { get { return Children[Children.Length - 1]; } }
		public bool IsLambda { get { return Name == null; } }
		public Type ReturnType { get { return Types.Dynamic; } }
		public override Type Type { get { return Types.Object; } }
		public LabelTarget ReturnLabel { get; internal set; }
		public List<Local> Locals { get; protected set; }

		public Lambda(INode name, List<string> parameters, INode body, ITree node)
			: base(NodeType.Lambda, node) {
			Variables = new Dictionary<string, Variable>();
			Locals = new List<Local>();

			Children = new INode[parameters.Count + 4];
			Children[0] = name;
			Children[Children.Length - 1] = body;

			ParamNames = ArrayUtils.Insert("~closure", "~this", ArrayUtils.MakeArray(parameters));

			// Setup ~closure
			CreateVar(ParamNames[0], new Parameter(ParamNames[0]));
			Children[1] = Var(ParamNames[0]);

			// Setup ~this
			CreateVar(ParamNames[1], new Parameter(ParamNames[1]));
			Children[2] = Var(ParamNames[1]);

			for (int i = 0; i < parameters.Count; ++i) {
				CreateVar(parameters[i], new Parameter(parameters[i]));
				Children[i + 3] = Var(parameters[i]);
			}
		}

		public override INode Analyze(Stack<Lambda> stack) {
			stack.Push(this);
			base.Analyze(stack);
			stack.Pop();
			return this;
		}

		internal Dictionary<string, Variable> Variables;
		public Variable Var(string name) { return Variables[name]; }
		public bool Var(string name, out Variable var) { return Variables.TryGetValue(name, out var); }
		public void CreateVar(string name, Variable var) {
			if (Variables.ContainsKey(name))
				throw new AstError("A variable named '" + name + "' already exist");

			if (var is Local)
				Locals.Add((Local)var);

			Variables[name] = var;
		}

		public override Expression Compile(Lambda func) {
			return AstTools.New(
				typeof(Function),
				AstTools.Constant(this),
				AstTools.New(
					typeof(Closure),
					CompileTools.Context(func),
					CompileTools.Globals(func)
				)
			);
		}
	}
}

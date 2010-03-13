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
using IronJS.Tools;
using IronJS.Ast.Tools;
using IronJS.Runtime.Js;
using IronJS.Runtime.Jit;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Nodes {
	public class Lambda : Base {
        public INode Name { get { return Children[0]; } }
        public INode Body { get { return Children[Children.Length - 1]; } }

		public Type ReturnType { get { return Types.Dynamic; } }
		public override Type Type { get { return Types.Object; } }
        public LabelTarget ReturnLabel { get; set; }

        public DelegateCache JitCache { get; private set; }
        public Scope Scope { get; private set; }

		public Lambda(INode name, List<string> paramNames, INode body, ITree node)
			: base(NodeType.Lambda, node) {

            Scope = new Scope();
            JitCache = new DelegateCache();

            // +4 for name, ~closure ,~this and body
			Children = new INode[paramNames.Count + 4]; 
			Children[0] = name;
            Children[1] = Scope.Add(Node.Parameter("~closure"));
            Children[2] = Scope.Add(Node.Parameter("~this"));
			Children[Children.Length - 1] = body;

            for (int i = 0; i < paramNames.Count; ++i) {
                Children[i + 3] = Scope.Add(Node.Parameter(paramNames[i]));
			}
		}

		public override INode Analyze(Stack<Lambda> stack) {
			stack.Push(this);
			base.Analyze(stack);
			stack.Pop();
			return this;
		}

		public override Expression Compile(Lambda func) {
            return AstTools.New(
                typeof(Obj),
                AstTools.New(
                    typeof(Closure),
                    AstTools.Constant(this),
                    AstTools.New(
                        typeof(ClosureCtx),
                        CompileTools.Context(func),
                        CompileTools.Globals(func)
                    )
                )
            );
		}
	}
}

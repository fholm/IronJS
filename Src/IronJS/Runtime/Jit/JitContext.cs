using System;
using System.Collections.Generic;
using System.Text;
using IronJS.Ast.Nodes;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Runtime.Jit {
	using Et = Expression;

	public class JitContext {
 
		public Lambda Lambda { get; private set; }

		public Et Globals {
			get {
				return Et.Field(Lambda.Children[1].Compile(this), "Globals");
			}
		}

		public Et Context {
			get {
				return Et.Field(Lambda.Children[1].Compile(this), "Context");
			}
		}

		public JitContext(Lambda lambda) {
			Lambda = lambda;
		}
	}
}

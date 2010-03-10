using System;
using System.Collections.Generic;
using System.Text;
using IronJS.Ast.Nodes;

namespace IronJS.Runtime.Jit {
	public class JitContext {
		public Lambda Lambda { get; private set; }
	}
}

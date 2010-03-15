using System;
using System.Collections.Generic;
using IronJS.Ast.Nodes;

namespace IronJS.Runtime.Js {
	public partial class ClosureCtx {
		public readonly Obj Globals;
		public readonly RuntimeCtx Runtime;
        public readonly LinkedList<Obj> DynamicScopes;

        public ClosureCtx(RuntimeCtx runtime, Obj globals) {
			Globals = globals;
			Runtime = runtime;
            DynamicScopes = new LinkedList<Obj>();
		}
    }

    public class ClosureCtxN : ClosureCtx {
        public object[] Items;

        public ClosureCtxN(RuntimeCtx runtime, Obj globals)
            : base(runtime, globals) { }
    }
}

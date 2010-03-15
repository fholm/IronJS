using System.Collections.Generic;

namespace IronJS.Runtime.Js {
	public class ClosureCtx {
		public readonly Obj Globals;
		public readonly RuntimeCtx Runtime;
        public readonly LinkedList<Obj> DynamicScopes;

        public ClosureCtx(RuntimeCtx context, Obj globals) {
			Globals = globals;
			Runtime = context;
            DynamicScopes = new LinkedList<Obj>();
		}
	}
}

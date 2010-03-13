using System.Collections.Generic;

namespace IronJS.Runtime.Js {
	public class ClosureCtx {
		public readonly Obj Globals;
		public readonly RuntimeCtx Runtime;
        public readonly List<Obj> DynamicScopes;

        public ClosureCtx(RuntimeCtx context, Obj globals) {
			Globals = globals;
			Runtime = context;
            DynamicScopes = new List<Obj>();
		}
	}
}

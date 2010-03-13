namespace IronJS.Runtime.Js {
	public class ClosureCtx {
		public readonly Obj Globals;
		public readonly RuntimeCtx Context;

        public ClosureCtx(RuntimeCtx context, Obj globals) {
			Globals = globals;
			Context = context;
		}
	}
}

namespace IronJS.Runtime.Js {
	public class Closure {
		public readonly Obj Globals;
		public readonly Context Context;

		public Closure(Context context, Obj globals) {
			Globals = globals;
			Context = context;
		}
	}
}

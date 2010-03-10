using System;

namespace IronJS.Runtime.Js {
	public static class Types {
		public static readonly Type Boolean = typeof(bool);
		public static readonly Type Integer = typeof(long);
		public static readonly Type Double = typeof(double);
		public static readonly Type String = typeof(string);
		public static readonly Type Object = typeof(Obj);
		public static readonly Type Dynamic = typeof(object);
		public static readonly Type Undefined = typeof(Undefined);
	}
}

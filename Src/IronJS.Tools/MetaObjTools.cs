using System;
using System.Dynamic;

namespace IronJS.Tools {
	using MetaObj = DynamicMetaObject;

	public static class MetaObjTools {
		public static Type[] GetTypes(params MetaObj[] objs) {
			return ArrayTools.Map(objs, delegate(MetaObj obj) {
				return obj.LimitType;
			});
		}
	}
}

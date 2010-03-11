using System;
using System.Runtime.CompilerServices;

namespace IronJS.Tools {
	public static class TypeTools {
		public static Type StrongBoxType = typeof(StrongBox<>);

		public static string ShortName(Type that) {
			string typeName = ArrayTools.First(
				ArrayTools.Last(
					that.Name.Split('.')
				).Split('`')
			);

			if (that.IsGenericType) {
				if (!that.IsGenericTypeDefinition) {
					string[] names = ArrayTools.Map(
						that.GetGenericArguments(), 
						delegate(Type type) {
							return ShortName(type);
						}
					);

					string generic = "<";
					foreach (string name in names) {
						generic += name + ",";
					}

					return typeName + generic.Substring(0, generic.Length - 1) + ">";
				}
			}

			return typeName;
		}
	}
}

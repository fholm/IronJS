using System.Collections.Generic;
using Microsoft.Scripting.Utils;

namespace IronJS.Tools {
	public static class IEnumerableTools {
		public static R[] Map<T, R>(IEnumerable<T> that, Func<T, R> func) {
			List<R> list = new List<R>();

			foreach (T x in that)
				list.Add(func(x));

			return list.ToArray();
		}
	}
}

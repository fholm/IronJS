using System;
using System.Collections.Generic;
using System.Dynamic;
using Microsoft.Scripting.Utils;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Tools {
	using MetaObj = DynamicMetaObject;

	public static class ArrayTools {
		public static T[] Filter<T>(T[] that, Func<T, bool> predicate) {
			List<T> list = new List<T>();

			for (int index = 0; index < that.Length; ++index)
				if (predicate(that[index]))
					list.Add(that[index]);

			return list.ToArray();
		}

		public static T Last<T>(T[] that) {
			if (that.Length == 0)
				throw new ArgumentException("Array is empty");

			return that[that.Length - 1];
		}

		public static T First<T>(T[] that) {
			if (that.Length == 0)
				throw new ArgumentException("Array is empty");

			return that[0];
		}

		public static O[] Map<T, O>(T[] that, Func<T, O> func) {
			O[] array = new O[that.Length];

			for (int index = 0; index < that.Length; ++index)
				array[index] = func(that[index]);

			return array;
		}

		public static T[] Concat<T>(T[] a, T[] b) {
			T[] array = new T[a.Length + b.Length];

			Array.Copy(a, array, a.Length);
			Array.Copy(b, 0, array, a.Length, b.Length);

			return array;
		}

		public static Type[] GetTypes(object[] that) {
			Type[] array = new Type[that.Length];

			for (int index = 0; index < that.Length; ++index)
				array[index] = that[index].GetType();

			return array;
		}

		public static T[] AddFirstAndLast<T>(T[] that, T first, T last) {
			return ArrayUtils.Insert(
				first,
				ArrayUtils.Append(
					that, last
				)
			);
		}

		public static T[] DropFirstAndLast<T>(T[] that) {
			return ArrayUtils.RemoveFirst(
				ArrayUtils.RemoveLast(that)
			);
		}

		public static Type[] GetLimitTypes(MetaObj[] that) {
			Type[] array = new Type[that.Length];

			for (int index = 0; index < that.Length; ++index)
				array[index] = that[index].LimitType;

			return array;
		}

		public static Type[] GetExpressionTypes(MetaObj[] that) {
			Type[] array = new Type[that.Length];

			for (int index = 0; index < that.Length; ++index)
				array[index] = that[index].Expression.Type;

			return array;
		}
	}
}

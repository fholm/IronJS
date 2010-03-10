using System.Collections.Generic;

namespace IronJS.Tools {
	public static class DictionaryTools {
		public static TKey[] GetKeys<TKey, TValue>(Dictionary<TKey, TValue> that) {
			TKey[] array = new TKey[that.Count];

			int index = 0;
			foreach (KeyValuePair<TKey, TValue> kvp in that)
				array[index++] = kvp.Key;

			return array;
		}

		public static TValue[] GetValues<TKey, TValue>(Dictionary<TKey, TValue> that) {
			TValue[] array = new TValue[that.Count];

			int index = 0;
			foreach (KeyValuePair<TKey, TValue> kvp in that)
				array[index++] = kvp.Value;

			return array;
		}
	}
}

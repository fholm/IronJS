using System;
using System.Collections.Generic;
using System.Text;

namespace IronJS.Tools
{
    public static class DictionaryTools
    {
        public static TKey[] GetKeys<TKey, TValue>(Dictionary<TKey, TValue> that)
        {
            TKey[] array = new TKey[that.Count];

            int n = 0;
            foreach (KeyValuePair<TKey, TValue> kvp in that)
                array[n++] = kvp.Key;

            return array;
        }

        public static TValue[] GetValues<TKey, TValue>(Dictionary<TKey, TValue> that)
        {
            TValue[] array = new TValue[that.Count];

            int n = 0;
            foreach (KeyValuePair<TKey, TValue> kvp in that)
                array[n++] = kvp.Value;

            return array;
        }
    }
}

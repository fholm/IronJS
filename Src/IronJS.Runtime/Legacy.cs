using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Runtime
{
#if LEGACY_HASHSET
    public class HashSet<T> : ICollection<T>
    {
        object slug = new object();
        Dictionary<T, object> storage = new Dictionary<T, object>();

        public void Add(T item)
        {
            this.storage[item] = slug;
        }

        public void Clear()
        {
            this.storage.Clear();
        }

        public bool Contains(T item)
        {
            return this.storage.ContainsKey(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            this.storage.Keys.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return this.storage.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(T item)
        {
            return this.storage.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return this.storage.Keys.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
#endif

#if LEGACY_DELEGATES
    public delegate void Action();
    public delegate void Action<in T>(T obj);
    public delegate void Action<in T1, in T2>(T1 arg1, T2 arg2);
    public delegate void Action<in T1, in T2, in T3>(T1 arg1, T2 arg2, T3 arg3);

    public delegate TResult Func<out TResult>();
    public delegate TResult Func<in T, out TResult>(T arg);
    public delegate TResult Func<in T1, in T2, out TResult>(T1 arg1, T2 arg2);
    public delegate TResult Func<in T1, in T2, in T3, out TResult>(T1 arg1, T2 arg2, T3 arg3);
    public delegate TResult Func<in T1, in T2, in T3, in T4, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
#endif

#if LEGACY_DELEGATES_HIGH_ARITY
    public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    public delegate TResult Func<in T1, in T2, in T3, in T4, in T5, in T6, out TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5, T6 arg6);
#endif

#if LEGACY_SORTED_DICT
    public class SortedDictionary<TKey, TValue>
    {
    }
#endif

#if LEGACY_BIGINT
    public static class BigIntegerParser
    {
        public static bool TryParse(string value, out System.Numerics.BigInteger result)
        {
            return TryParse(value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out result);
        }

        public static bool TryParse(string value, System.Globalization.NumberStyles style, System.Globalization.CultureInfo culture, out System.Numerics.BigInteger result)
        {
            try
            {
                result = System.Numerics.BigInteger.Parse(value);
                return true;
            }
            catch (FormatException)
            {
                result = System.Numerics.BigInteger.Zero;
                return false;
            }
        }
    }
#endif
}

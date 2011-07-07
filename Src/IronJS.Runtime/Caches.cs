using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Runtime.Caches
{
    public class LimitCache<K, V>
        where K : IEquatable<K>
        where V : class
    {
        int size;
        List<Tuple<K, V>> storage =
            new List<Tuple<K, V>>();

        public LimitCache(int halfSize)
        {
            this.size = halfSize * 2;
        }

        public V Lookup(K key, Lazy<V> value)
        {
            var result = storage.FirstOrDefault(x => x.Item1.Equals(key));

            if (result == null)
            {
                storage.Add(Tuple.Create(key, value.Value));

                if (storage.Count > size)
                {
                    storage = storage.Take(size).ToList();
                }
            }

            return result.Item2;
        }
    }

    public class SplayCache<K, V>
        where K : IComparable<K>
    {
        SplayTree<K, V> storage =
            new SplayTree<K, V>();

        public V Lookup(K key, Lazy<V> value)
        {
            V cached;

            if (!storage.TryGetValue(key, out cached))
            {
                storage[key] = cached = value.Value;

                if (storage.Count > 1024)
                {
                    storage.Trim(10);
                }
            }

            return cached;
        }
    }

    public class WeakCache<K, V>
    {
        Dictionary<K, WeakReference> storage =
            new Dictionary<K, WeakReference>();

        public V Lookup(K key, Func<V> value)
        {
            WeakReference cached;

            if (storage.TryGetValue(key, out cached) && cached.Target is V)
            {
                return (V)cached.Target;
            }

            var newValue = value();
            storage[key] = new WeakReference(newValue);
            return newValue;
        }
    }
}

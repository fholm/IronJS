using System;
using System.Collections.Generic;

namespace Clr2Support {
    public class ConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue> {
        object _sync;
        Dictionary<TKey, TValue> _values;

        public ConcurrentDictionary() {
            _sync = new object();
            _values = new Dictionary<TKey, TValue>();
        }

        public bool TryAdd(TKey key, TValue value) {
            lock (_sync) {
                if (_values.ContainsKey(key)) {
                    return false;
                }
                _values.Add(key, value);
                return true;
            }
        }

        public void Add(TKey key, TValue value) {
            _values.Add(key, value);
        }

        public bool ContainsKey(TKey key) {
            return _values.ContainsKey(key);
        }

        public ICollection<TKey> Keys {
            get { return _values.Keys; }
        }

        public bool Remove(TKey key) {
            return _values.Remove(key);
        }

        public bool TryGetValue(TKey key, out TValue value) {
            lock (_sync) {
                return _values.TryGetValue(key, out value);
            }
        }

        public ICollection<TValue> Values {
            get { return _values.Values; }
        }

        public TValue this[TKey key] {
            get {
                return _values[key];
            }
            set {
                _values[key] = value;
            }
        }

        public void Clear() {
            _values.Clear();
        }

        public int Count {
            get { return _values.Count; }
        }

        public bool IsReadOnly {
            get { return false; }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() {
            return _values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() {
            return _values.GetEnumerator();
        }

        public void Add(KeyValuePair<TKey, TValue> item) {
            throw new NotImplementedException();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item) {
            throw new NotImplementedException();
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex) {
            throw new NotImplementedException();
        }

        public bool Remove(KeyValuePair<TKey, TValue> item) {
            throw new NotImplementedException();
        }
    }
}

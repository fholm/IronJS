using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Runtime
{
    public class ArrayObject : CommonObject
    {
        public const uint DenseMaxIndex = 2147483646u;
        public const uint DenseMaxSize = 2147483647u;


        public class SparseArray
        {
            SortedDictionary<uint, BoxedValue> storage =
                new SortedDictionary<uint, BoxedValue>();

            public SortedDictionary<uint, BoxedValue> Members
            {
                get { return this.storage; }
            }

            public void Put(uint index, BoxedValue value)
            {
                this.storage[index] = value;
            }

            public bool Has(uint index)
            {
                return this.storage.ContainsKey(index);
            }

            public BoxedValue Get(uint index)
            {
                return this.storage[index];
            }

            public bool TryGet(uint index, out BoxedValue value)
            {
                return this.storage.TryGetValue(index, out value);
            }

            public bool Remove(uint index)
            {
                return this.storage.Remove(index);
            }

            public void PutLength(uint newLength, uint length)
            {
                if (newLength >= length)
                {
                    return;
                }

                foreach (var key in this.storage.Keys)
                {
                    if (key >= newLength)
                        this.storage.Remove(key);
                }
            }

            public void Shift()
            {
                this.storage.Remove(0);

                foreach (var key in this.storage.Keys)
                {
                    var value = this.storage[key];
                    this.storage.Remove(key);
                    this.storage.Add(key, value); //FIXME: Isn't this supposed to subtract one from the key?  This probably isn't caught by our tests, because they don't trigger sparse arrays.
                }
            }

            public void Reverse(uint length)
            {
                var newStorage = new SortedDictionary<uint, BoxedValue>();

                foreach (var kvp in this.storage)
                {
                    newStorage.Add(length - kvp.Key - 1, kvp.Value);  //FIXME: What do we do when length > max(key)?  Does this just allow negative indices?
                }

                this.storage = newStorage;
            }

            public void Sort(Comparison<BoxedValue> comparison)
            {
                var sorted = this.storage.Values.ToArray();
                Array.Sort(sorted, comparison);

                this.storage.Clear();
                for (uint i = 0; i < sorted.Length; i++)
                {
                    this.storage.Add(i, sorted[i]);
                }
            }

            public void Unshift(BoxedValue[] args)
            {
                var length = (uint)args.Length;

                var newStorage = new SortedDictionary<uint, BoxedValue>();

                foreach (var kvp in this.storage)
                {
                    newStorage.Add(kvp.Key + length, kvp.Value);
                }

                for (uint i = 0; i < length; i++)
                {
                    newStorage.Add(i, args[i]);
                }

                this.storage = newStorage;
            }

            public void GetAllIndexProperties(IDictionary<uint, BoxedValue> dict, uint length)
            {
                foreach (var kvp in this.storage)
                {
                    if ((kvp.Key < length) && !dict.ContainsKey(kvp.Key))
                    {
                        dict.Add(kvp.Key, kvp.Value);
                    }
                }
            }

            public static SparseArray OfDense(Descriptor[] values)
            {
                var sparse = new SparseArray();

                for (int i = 0; i < values.Length; i++)
                {
                    if (values[i].HasValue)
                    {
                        uint num = (uint)i;
                        sparse.storage[num] = values[i].Value;
                    }
                }

                return sparse;
            }
        }
    }
}

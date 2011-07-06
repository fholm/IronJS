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
    }

    //TODO: Not done yet
    public class SparseArray
    {
        SortedDictionary<uint, BoxedValue> storage =
            new SortedDictionary<uint, BoxedValue>();

        public SortedDictionary<uint, BoxedValue> Members
        {
            get { return storage; }
        }

        public void Put(uint index, BoxedValue value)
        {
            storage[index] = value;
        }

        public bool Has(uint index)
        {
            return storage.ContainsKey(index);
        }

        public BoxedValue Get(uint index)
        {
            return storage[index];
        }

        public bool TryGet(uint index, out BoxedValue value)
        {
            return storage.TryGetValue(index, out value);
        }

        public bool Remove(uint index)
        {
            return storage.Remove(index);
        }
    }
}

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

        private Descriptor[] dense;
        private SparseArray sparse;
        private bool isDense;
        private uint length;

        public ArrayObject(Environment env, uint length) :
            base(env, env.Maps.Array, env.Prototypes.Array)
        {
            this.length = length;

            if (length > 0x20000)
            {
                this.sparse = new SparseArray();
                this.isDense = false;
            }
            else
            {
                this.dense = new Descriptor[length];
                this.isDense = true;
            }
        }

        private void ResizeDense(uint newCapacity)
        {
            var capacity = (uint)this.dense.Length;
            newCapacity = newCapacity == 0 ? 2 : newCapacity;

            var copyLength = newCapacity < capacity ? newCapacity : capacity;

            var newDense = new Descriptor[newCapacity];
            Array.Copy(this.dense, newDense, copyLength);
            this.dense = newDense;
        }

        public Descriptor[] Dense
        {
            get { return this.dense; }
            set { this.dense = value; }
        }

        public SparseArray Sparse
        {
            get
            {
                return this.sparse;
            }
        }

        public override uint Length
        {
            get
            {
                return this.length;
            }
            set
            {
                this.length = value;
                base.Put("length", (double)value, DescriptorAttrs.NotEnumerable);
            }
        }

        public override string ClassName
        {
            get { return "Array"; }
        }

        public bool IsDense { get { return this.isDense; } }

        public override void GetAllIndexProperties(IDictionary<uint, BoxedValue> dict, uint l)
        {
            if (this.isDense)
            {
                int length = (int)this.length;
                for (int i = 0; i < length; i++)
                {
                    if (((i < l) && this.dense[i].HasValue) && !dict.ContainsKey((uint)i))
                    {
                        dict.Add((uint)i, this.dense[i].Value);
                    }
                }
            }
            else
            {
                //FIXME:  Was this function supposed to use the `l` parameter or the `this.length` to limit the entries?
                this.sparse.GetAllIndexProperties(dict, this.length);
            }
        }

        internal bool HasIndex(uint index)
        {
            if (index >= this.length)
            {
                return false;
            }

            if (!this.isDense)
            {
                return this.sparse.storage.ContainsKey(index);
            }

            return ((index < this.dense.Length) && this.dense[index].HasValue);
        }

        private void PutLength(uint newLength)
        {
            if (this.isDense)  //INFO: Shouldn't we just clip or grow the dense array to the proper value when the user asks us to?
            {
                while (newLength < this.length)  //FIXME: What if we are larger than the dense array, shouldn't we grow the array?
                {
                    this.length--;
                    if (this.length < this.dense.Length)
                    {
                        this.dense[this.length].Value = new BoxedValue();
                        this.dense[this.length].HasValue = false;
                    }
                }
            }
            else
            {
                this.sparse.PutLength(newLength, this.length);
            }

            this.length = newLength;
            base.Put("length", (double)newLength);
        }

        internal void PutLength(double newLength)
        {
            uint length = (uint)newLength;
            if ((newLength >= 0.0) ? (length != newLength) : true)
            {
                this.Env.RaiseRangeError<object>("Invalid array length");
                return;
            }
            this.PutLength(length);
        }

        public override void Put(uint index, BoxedValue value)
        {
            if (index == uint.MaxValue)
            {
                base.Put(index.ToString(), value);
                return;
            }

            if (this.isDense)
            {
                var denseLength = (uint)this.dense.Length;

                if (index < denseLength)
                {
                    this.dense[index].Value = value;
                    this.dense[index].HasValue = true;
                    if (index >= this.length)
                    {
                        this.Length = index + 1;
                    }
                    return;
                }
                else if (index < (denseLength + 10))
                {
                    // We're above the currently allocated dense size
                    // but not far enough above to switch to sparse
                    // so we expand the dense array
                    this.ResizeDense(denseLength * 2 + 10);
                    this.dense[index].Value = value;
                    this.dense[index].HasValue = true;
                    this.Length = index + 1;
                    return;
                }
                else
                {
                    // Switch to sparse array
                    this.sparse = SparseArray.OfDense(this.dense);
                    this.dense = null;
                    this.isDense = false;

                    // Fall through to the sparse handling below.
                }
            }

            sparse.Put(index, value);
            if (index >= length)
            {
                this.Length = index + 1;  //INFO: I changed this from setting the field to setting the property, like above.  This means that it stores the value on the base object as well.
            }
        }

        public override void Put(uint index, double value)
        {
            this.Put(index, BoxedValue.Box(value));
        }

        public override void Put(uint index, object value, uint tag)
        {
            this.Put(index, BoxedValue.Box(value, tag));
        }

        public override void Put(string name, BoxedValue value)
        {
            if (name == "length")
            {
                this.PutLength(TypeConverter.ToNumber(value));
                this.SetAttrs("length", DescriptorAttrs.NotEnumerable); //TODO: Shouldn't `PutLength` keep the `DontEnum` flag?
                return;
            }

            uint index;
            if (TypeConverter.TryToIndex(name, out index))  //TODO: I changed this to use TryToIndex, but that forgoes checking that `index.ToString() == name`, which may be necessary.
            {
                this.Put(index, value);
                return;
            }

            base.Put(name, value);
        }

        public override void Put(string name, double value)
        {
            if (name == "length")
            {
                this.PutLength(TypeConverter.ToNumber(value));
                this.SetAttrs("length", DescriptorAttrs.NotEnumerable); //TODO: Shouldn't `PutLength` keep the `DontEnum` flag?
                return;
            }

            uint index;
            if (TypeConverter.TryToIndex(name, out index))  //TODO: I changed this to use TryToIndex, but that forgoes checking that `index.ToString() == name`, which may be necessary.
            {
                this.Put(index, value);
                return;
            }

            base.Put(name, value);
        }

        public override void Put(string name, object value, uint tag)
        {
            var boxed = BoxedValue.Box(value, tag);

            if (name == "length")
            {
                this.PutLength(TypeConverter.ToNumber(boxed));
                this.SetAttrs("length", DescriptorAttrs.NotEnumerable); //TODO: Shouldn't `PutLength` keep the `DontEnum` flag?
                return;
            }

            uint index;
            if (TypeConverter.TryToIndex(name, out index))  //TODO: I changed this to use TryToIndex, but that forgoes checking that `index.ToString() == name`, which may be necessary.
            {
                this.Put(index, boxed);
                return;
            }

            base.Put(name, boxed);
        }

        public override BoxedValue Get(string name)
        {
            uint index;
            if (uint.TryParse(name, out index))
            {
                return this.Get(index);
            }

            if (string.Equals(name, "length"))
            {
                return BoxedValue.Box(this.length);
            }

            return base.Get(name);
        }

        public override BoxedValue Get(uint index)
        {
            if (index == uint.MaxValue)
            {
                return base.Get(index.ToString());
            }

            if (this.HasIndex(index))
            {
                if (this.isDense)
                {
                    return dense[index].Value;
                }
                else
                {
                    return sparse.Get(index);
                }
            }

            return this.Prototype.Get(index);

            //TODO: This was ported from the following F# code, but I simplified it.  It may have broken due to the simplification.
            ////let ii = int index
            ////if isDense && ii >= 0 && ii < dense.Length && dense.[ii].HasValue then
            ////    dense.[ii].Value
            ////else
            ////    if index = UInt32.MaxValue then
            ////        base.Get(string index)
            ////    else
            ////        if x.HasIndex(index) then
            ////            if isDense 
            ////                then dense.[int index].Value
            ////                else sparse.Get(index)
            ////        else
            ////            x.Prototype.Get(index)
        }

        public override bool Has(string name)
        {
            uint index;
            if (uint.TryParse(name, out index))
            {
                return this.Has(index);
            }

            return base.Has(name);
        }

        public override bool Has(uint index)
        {
            if (index == uint.MaxValue)
            {
                return base.Has(index.ToString());
            }

            return this.HasIndex(index) || this.Prototype.Has(index);
        }

        public override bool HasOwn(string name)
        {
            uint index;
            if (uint.TryParse(name, out index))
            {
                return this.HasOwn(index);
            }

            return base.HasOwn(name);
        }

        public override bool HasOwn(uint index)
        {
            if (index == uint.MaxValue)
            {
                return base.HasOwn(index.ToString());
            }

            return this.HasIndex(index);
        }

        public override bool Delete(string name)
        {
            uint index;
            if (uint.TryParse(name, out index))
            {
                return this.Delete(index);
            }

            return base.Delete(name);
        }

        public override bool Delete(uint index)
        {
            if (index == uint.MaxValue)
            {
                return base.Delete(index.ToString());
            }

            if (this.HasIndex(index))
            {
                if (this.isDense)
                {
                    //TODO: This was creating a new boxed value and pushing it into the existing descriptor, along with a `false` for has value.
                    //  I changed it to a zero-initialized descriptor for speed and clarity.
                    this.dense[index] = default(Descriptor);
                }
                else
                {
                    return sparse.Remove(index);
                }
            }

            return false;
        }

        public class SparseArray
        {
            internal SortedDictionary<uint, BoxedValue> storage = new SortedDictionary<uint, BoxedValue>();

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

                foreach (var key in this.storage.Keys.ToList())
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

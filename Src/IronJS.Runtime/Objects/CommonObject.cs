using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;

namespace IronJS.Runtime
{
    public class CommonObject : DynamicObject
    {
        public Environment Env;
        public CommonObject Prototype;
        public Schema PropertySchema;
        public Descriptor[] Properties;

        public CommonObject(Environment env, Schema map, CommonObject prototype)
        {
            this.Env = env;
            this.Prototype = prototype;
            this.PropertySchema = map;
            this.Properties = new Descriptor[map.IndexMap.Count];
        }


        public CommonObject(Environment env, CommonObject prototype)
        {
            this.Env = env;
            this.Prototype = prototype;
            this.PropertySchema = env.Maps.Base;
            this.Properties = new Descriptor[env.Maps.Base.IndexMap.Count];
        }

        internal CommonObject(Environment env)
        {
            this.Env = env;
            this.Prototype = null;
            this.PropertySchema = null;
            this.Properties = null;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            var prop = this.Get(binder.Name);
            result = prop.UnboxObject();
            return true;
        }

        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            if (indexes.Length != 1)
            {
                result = null;
                return false;
            }

            var prop = this.Get(indexes[0].ToString());
            result = prop.UnboxObject();
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this.Put(binder.Name, value);
            return true;
        }

        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (indexes.Length != 1)
                return false;

            this.Put(indexes[0].ToString(), value);
            return true;
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            var item = this.Find(binder.Name);
            if (item.HasValue)
            {
                var box = item.Value;
                if (box.IsFunction)
                {
                    var func = box.Func;
                    var boxedArgs = args.Select(a => BoxedValue.Box(a)).ToArray();
                    var ret = func.Call(this, boxedArgs);
                    result = ret.UnboxObject();
                    return true;
                }
            }

            result = null;
            return false;
        }

        public override string ToString()
        {
            var item = this.Find("toString");
            if (item.HasValue)
            {
                var box = item.Value;
                if (box.IsFunction)
                {
                    var func = box.Func;
                    var ret = func.Call(this);
                    TypeConverter.ToString(ret);
                }
            }

            return base.ToString();
        }

        public virtual string ClassName { get { return "Object"; } }

        public string ClassType { get { return this.GetType().Name; } }

        public Dictionary<string, object> Members()
        {
            var dict = new Dictionary<string, object>();
            foreach (var kvp in this.PropertySchema.IndexMap)
            {
                if (this.Properties[kvp.Value].HasValue)
                {
                    dict.Add(kvp.Key, this.Properties[kvp.Value].Value.ClrBoxed);
                }
            }

            return dict;
        }

        public override IEnumerable<string> GetDynamicMemberNames()
        {
            foreach (var kvp in this.PropertySchema.IndexMap)
            {
                yield return kvp.Key;
            }
        }

        public bool HasPrototype
        {
            get { return this.Prototype != null; }
        }

        public int RequiredStorage
        {
            get { return this.PropertySchema.IndexMap.Count; }
        }

        // Default implementation of Length used for
        // all objects except ArrayObject
        public virtual uint Length
        {
            get { return TypeConverter.Touint(this.Get("length")); }
            set { this.Put("length", value); }
        }

        public virtual void GetAllIndexProperties(Dictionary<uint, BoxedValue> dict, uint length)
        {
            var i = 0u;

            foreach (var kvp in this.PropertySchema.IndexMap)
            {
                if (uint.TryParse(kvp.Key, &i) && i < length && !dict.ContainsKey(i) && this.Properties[kvp.Value].HasValue)
                {
                    dict.Add(i, this.Properties[kvp.Value].Value);
                }
            }

            if (this.Prototype != null)
            {
                this.Prototype.GetAllIndexProperties(dict, length);
            }
        }

        public T CastTo<T>() where T : CommonObject
        {
            var o = this as T;
            if (o != null)
            {
                return o;
            }

            this.Env.RaiseTypeError("Could not cast " + this.GetType().Name + " to " + typeof(T).Name);
        }

        public bool TryCastTo<T>(out T result) where T : CommonObject
        {
            var o = this as T;
            if (o != null)
            {
                result = o;
                return true;
            }

            result = null;
            return false;
        }

        public void CheckType<T>() where T : CommonObject
        {
            if (!(this is T))
            {
                this.Env.RaiseTypeError(this.GetType().Name + " is not " + typeof(T).Name);
            }
        }

        /// <summary>
        /// Expands object property storage
        /// </summary>
        public void ExpandStorage()
        {
            var newValues = new Descriptor[this.RequiredStorage * 2];

            if (this.Properties.Length > 0)
            {
                Array.Copy(this.Properties, newValues, this.Properties.Length);
            }

            this.Properties = newValues;

        }

        /// <summary>
        /// Creates an index for property named <paramref name="name"/>
        /// </summary>
        /// <param name="name">The property to be allocated.</param>
        internal int CreateIndex(string name)
        {
            this.PropertySchema = this.PropertySchema.SubClass(name);

            if (this.RequiredStorage >= this.Properties.Length)
            {
                this.ExpandStorage();
            }

            return this.PropertySchema.IndexMap[name];
        }

        /// <summary>
        /// Finds a property in the prototype chain.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        protected Descriptor Find(string name)
        {
            return Find(this, name);
        }

        private static Descriptor Find(CommonObject @this, string name)
        {
            while (@this != null)
            {
                int index;
                if (@this.PropertySchema.IndexMap.TryGetValue(name, out index))
                {
                    return @this.Properties[index];
                }

                @this = @this.Prototype;
            }

            return new Descriptor();
        }

        /// <summary>
        /// Can we put property named <paramref name="name"/>?
        /// </summary>
        /// <param name="name"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        private bool CanPut(string name, out int index)
        {
            if (this.PropertySchema.IndexMap.TryGetValue(name, out index))
            {
                return this.Properties[index].IsWritable;
            }
            else
            {
                var loop = true;
                var cobj = this.Prototype;

                while (loop && cobj != null)
                {
                    if (cobj.PropertySchema.IndexMap.TryGetValue(name, out index))
                    {
                        loop = false;
                    }
                    else
                    {
                        cobj = cobj.Prototype;
                    }
                }

                if (cobj == null || cobj.Properties[index].IsWritable)
                {
                    index = this.CreateIndex(name);
                    return true;
                }

                return false;
            }
        }

        public void SetAttrs(string name, ushort attrs)
        {
            int index = 0;
            if (this.PropertySchema.IndexMap.TryGetValue(name, out index))
            {
                this.Properties[index].Attributes |= attrs;
            }
        }

        public BoxedValue? TryCallMember(string name)
        {
            BoxedValue box = this.Get(name);
            if (box.IsFunction)
            {
                return box.Func.Call(this);
            }

            return null;
        }

        public BoxedValue CallMember(string name)
        {
            return this.Get(name).Func.Call(this);
        }

        //----------------------------------------------------------------------------
        // These methods are the core Put/Get/Has/Delete methods for property access
        //----------------------------------------------------------------------------

        public virtual void Put(string name, BoxedValue value)
        {
            int index = 0;
            if (this.CanPut(name, out index))
            {
                this.Properties[index].Value = value;
                this.Properties[index].HasValue = true;
            }
        }

        public virtual void Put(string name, object value, uint tag)
        {
            int index = 0;
            if (this.CanPut(name, out index))
            {
                this.Properties[index].Value.Clr = value;
                this.Properties[index].Value.Tag = tag;
                this.Properties[index].HasValue = true;
            }
        }

        public virtual void Put(string name, double value)
        {
            int index = 0;
            if (this.CanPut(name, out index))
            {
                this.Properties[index].Value.Number = value;
                this.Properties[index].HasValue = true;
            }
        }

        public virtual BoxedValue Get(string name)
        {
            Descriptor descriptor = this.Find(name);
            if (descriptor.HasValue)
            {
                return descriptor.Value;
            }
            return Undefined.Boxed;
        }

        public virtual bool Has(string name)
        {
            return this.Find(name).HasValue;
        }

        public virtual bool HasOwn(string name)
        {
            int index = 0;
            return this.PropertySchema.IndexMap.TryGetValue(name, out index) && this.Properties[index].HasValue;
        }

        public virtual bool Delete(string name)
        {
            int index = 0;
            if (!this.PropertySchema.IndexMap.TryGetValue(name, out index))
            {
                return true;
            }

            if ((this.Properties[index].Attributes & DescriptorAttrs.DontDelete) == DescriptorAttrs.None)
            {
                this.PropertySchema = this.PropertySchema.Delete(name);
                this.Properties[index] = new Descriptor();
                return true;
            }

            return false;
        }

        //----------------------------------------------------------------------------

        public virtual void Put(uint index, BoxedValue value)
        {
            this.Put(index.ToString(), value);
        }

        public virtual void Put(uint index, object value, uint tag)
        {
            this.Put(index.ToString(), value, tag);
        }

        public virtual void Put(uint index, double value)
        {
            this.Put(index.ToString(), value);
        }

        public virtual BoxedValue Get(uint index)
        {
            return this.Get(index.ToString());
        }

        public virtual bool Has(uint index)
        {
            return this.Has(index.ToString());
        }

        public virtual bool HasOwn(uint index)
        {
            return this.HasOwn(index.ToString());
        }

        public virtual bool Delete(uint index)
        {
            return this.Delete(index.ToString());
        }

        public virtual BoxedValue DefaultValue(DefaultValueHint hint)
        {
            BoxedValue? val;

            switch (hint)
            {
                case DefaultValueHint.String:
                    val = this.TryCallMember("toString");
                    if (val != null && val.Value.IsPrimitive)
                    {
                        return val.Value;
                    }

                    val = this.TryCallMember("valueOf");
                    if (val != null && val.Value.IsPrimitive)
                    {
                        return val.Value;
                    }

                    this.Env.RaiseTypeError("Could not get the default value.");
                    break;

                default:
                    val = this.TryCallMember("valueOf");
                    if (val != null && val.Value.IsPrimitive)
                    {
                        return val.Value;
                    }

                    val = this.TryCallMember("toString");
                    if (val != null && val.Value.IsPrimitive)
                    {
                        return val.Value;
                    }

                    this.Env.RaiseTypeError("Could not get the default value.");
                    break;
            }
        }

        public void Put(string name, bool value)
        {
            this.Put(name, value ? TaggedBools.True : TaggedBools.False);
        }

        public void Put(string name, object value)
        {
            this.Put(name, value, TypeTags.Clr);
        }

        public void Put(string name, string value)
        {
            this.Put(name, value, TypeTags.String);
        }

        public void Put(string name, Undefined value)
        {
            this.Put(name, value, TypeTags.Undefined);
        }

        public void Put(string name, CommonObject value)
        {
            this.Put(name, value, TypeTags.Object);
        }

        public void Put(string name, FunctionObject value)
        {
            this.Put(name, value, TypeTags.Function);
        }

        public void Put(uint index, bool value)
        {
            this.Put(index, value ? TaggedBools.True : TaggedBools.False);
        }

        public void Put(uint index, object value)
        {
            this.Put(index, value, TypeTags.Clr);
        }

        public void Put(uint index, string value)
        {
            this.Put(index, value, TypeTags.String);
        }

        public void Put(uint index, Undefined value)
        {
            this.Put(index, value, TypeTags.Undefined);
        }

        public void Put(uint index, CommonObject value)
        {
            this.Put(index, value, TypeTags.Object);
        }

        public void Put(uint index, FunctionObject value)
        {
            this.Put(index, value, TypeTags.Function);
        }

        public void Put(string name, BoxedValue value, ushort attrs)
        {
            this.Put(name, value);
            this.SetAttrs(name, attrs);
        }

        public void Put(string name, bool value, ushort attrs)
        {
            this.Put(name, value);
            this.SetAttrs(name, attrs);
        }

        public void Put(string name, double value, ushort attrs)
        {
            this.Put(name, value);
            this.SetAttrs(name, attrs);
        }

        public void Put(string name, object value, ushort attrs)
        {
            this.Put(name, value);
            this.SetAttrs(name, attrs);
        }

        public void Put(string name, string value, ushort attrs)
        {
            this.Put(name, value);
            this.SetAttrs(name, attrs);
        }

        public void Put(string name, Undefined value, ushort attrs)
        {
            this.Put(name, value);
            this.SetAttrs(name, attrs);
        }

        public void Put(string name, CommonObject value, ushort attrs)
        {
            this.Put(name, value);
            this.SetAttrs(name, attrs);
        }

        public void Put(string name, FunctionObject value, ushort attrs)
        {
            this.Put(name, value);
            this.SetAttrs(name, attrs);
        }

        //----------------------------------------------------------------------------
        // Put methods for setting indexes to BoxedValues
        //----------------------------------------------------------------------------

        public void Put(BoxedValue index, BoxedValue value)
        {
            uint i = 0;
            if (TypeConverter.TryToIndex(index, out i))
            {
                this.Put(i, value);
            }
            else
            {
                this.Put(TypeConverter.ToString(index), value);
            }
        }

        public void Put(bool index, BoxedValue value)
        {
            this.Put(TypeConverter.ToString(index), value);
        }

        public void Put(double index, BoxedValue value)
        {
            uint parsed = 0;
            if (TypeConverter.TryToIndex(index, out parsed))
            {
                this.Put(parsed, value);
            }
            else
            {
                this.Put(TypeConverter.ToString(index), value);
            }
        }

        public void Put(object index, BoxedValue value)
        {
            string s = TypeConverter.ToString(index);
            uint parsed = 0;
            if (TypeConverter.TryToIndex(s, ref parsed))
            {
                this.Put(parsed, value);
            }
            else
            {
                this.Put(s, value);
            }
        }

        public void Put(Undefined index, BoxedValue value)
        {
            this.Put("undefined", value);
        }

        public void Put(CommonObject index, BoxedValue value)
        {
            string s = TypeConverter.ToString(index);
            uint parsed = 0;
            if (TypeConverter.TryToIndex(s, ref parsed))
            {
                this.Put(parsed, value);
            }
            else
            {
                this.Put(s, value);
            }
        }

        //----------------------------------------------------------------------------
        // Put methods for setting indexes to doubles
        //----------------------------------------------------------------------------

        public void Put(BoxedValue index, double value)
        {
            uint i = 0;
            if (TypeConverter.TryToIndex(index, out i))
            {
                this.Put(i, value);
            }
            else
            {
                this.Put(TypeConverter.ToString(index), value);
            }
        }

        public void Put(bool index, double value)
        {
            this.Put(TypeConverter.ToString(index), value);
        }

        public void Put(double index, double value)
        {
            uint parsed = 0;
            if (TypeConverter.TryToIndex(index, out parsed))
            {
                this.Put(parsed, value);
            }
            else
            {
                this.Put(TypeConverter.ToString(index), value);
            }
        }

        public void Put(object index0, double value)
        {
            string index = TypeConverter.ToString(index0);
            uint parsed = 0;
            if (TypeConverter.TryToIndex(index, out parsed))
            {
                this.Put(parsed, value);
            }
            else
            {
                this.Put(index, value);
            }
        }

        public void Put(Undefined index, double value)
        {
            this.Put("undefined", value);
        }

        public void Put(CommonObject index, double value)
        {
            string s = TypeConverter.ToString(index);
            uint parsed = 0;
            if (TypeConverter.TryToIndex(s, out parsed))
            {
                this.Put(parsed, value);
            }
            else
            {
                this.Put(s, value);
            }
        }

        //----------------------------------------------------------------------------
        // Put methods for setting indexes to doubles
        //----------------------------------------------------------------------------

        public void Put(BoxedValue index, object value, ushort tag)
        {
            uint i = 0;
            if (TypeConverter.TryToIndex(index, ref i))
            {
                this.Put(i, value, tag);
            }
            else
            {
                this.Put(TypeConverter.ToString(index), value, tag);
            }
        }

        public void Put(bool index, object value, ushort tag)
        {
            this.Put(TypeConverter.ToString(index), value, tag);
        }

        public void Put(double index, object value, ushort tag)
        {
            uint parsed = 0;
            if (TypeConverter.TryToIndex(index, ref parsed))
            {
                this.Put(parsed, value, tag);
            }
            else
            {
                this.Put(TypeConverter.ToString(index), value, tag);
            }
        }

        public void Put(object index, object value, ushort tag)
        {
            var s = TypeConverter.ToString(index);
            var parsed = 0u;

            if (TypeConverter.TryToIndex(index, out parsed))
            {
                this.Put(parsed, value, tag);
            }
            else
            {
                this.Put(s, value, tag);
            }
        }

        public void Put(Undefined index, object value, ushort tag)
        {
            this.Put("undefined", value, tag);
        }

        public void Put(CommonObject index, object value, ushort tag)
        {
            string s = TypeConverter.ToString(index);
            uint parsed = 0;
            if (TypeConverter.TryToIndex(s, ref parsed))
            {
                this.Put(parsed, value, tag);
            }
            else
            {
                this.Put(s, value, tag);
            }
        }

        //----------------------------------------------------------------------------
        // Overloaded .Get methods that convert their argument into either a string
        // or uint32 and forwards the call to the correct .Get method
        //----------------------------------------------------------------------------

        public BoxedValue Get(BoxedValue index)
        {
            uint i = 0;
            if (TypeConverter.TryToIndex(index, ref i))
            {
                return this.Get(i);
            }
            return this.Get(TypeConverter.ToString(index));
        }

        public BoxedValue Get(bool index)
        {
            return this.Get(TypeConverter.ToString(index));
        }

        public BoxedValue Get(double index)
        {
            uint parsed = 0;
            if (TypeConverter.TryToIndex(index, ref parsed))
            {
                return this.Get(parsed);
            }
            return this.Get(TypeConverter.ToString(index));
        }

        public BoxedValue Get(object index)
        {
            string s = TypeConverter.ToString(index);
            uint parsed = 0;
            if (TypeConverter.TryToIndex(s, ref parsed))
            {
                return this.Get(parsed);
            }
            return this.Get(s);
        }

        public BoxedValue Get(Undefined index)
        {
            return this.Get("undefined");
        }

        public BoxedValue Get(CommonObject index)
        {
            string s = TypeConverter.ToString(index);
            uint parsed = 0;
            if (TypeConverter.TryToIndex(s, ref parsed))
            {
                return this.Get(parsed);
            }
            return this.Get(s);
        }

        public T GetT<T>(string name)
        {
            return this.Get(name).Unbox<T>();
        }

        public T GetT<T>(uint index)
        {
            return this.Get(index).Unbox<T>();
        }

        //----------------------------------------------------------------------------
        // Overloaded .Has methods that convert their argument into either a string
        // (property) or uint32 (index) and fowards the call to the correct .Has
        //----------------------------------------------------------------------------

        public bool Has(BoxedValue index)
        {
            uint i = 0;
            if (TypeConverter.TryToIndex(index, ref i))
            {
                return this.Has(i);
            }
            return this.Has(TypeConverter.ToString(index));
        }

        public bool Has(bool index)
        {
            return this.Has(TypeConverter.ToString(index));
        }

        public bool Has(double index)
        {
            uint parsed = 0;
            if (TypeConverter.TryToIndex(index, out parsed))
            {
                return this.Has(parsed);
            }
            return this.Has(TypeConverter.ToString(index));
        }

        public bool Has(object index)
        {
            string s = TypeConverter.ToString(index);
            uint parsed = 0;
            if (TypeConverter.TryToIndex(s, out parsed))
            {
                return this.Has(parsed);
            }
            return this.Has(s);
        }

        public bool Has(Undefined index)
        {
            return this.Has("undefined");
        }

        public bool Has(CommonObject index)
        {
            string s = TypeConverter.ToString(index);
            uint parsed = 0;
            if (TypeConverter.TryToIndex(s, ref parsed))
            {
                return this.Has(parsed);
            }
            return this.Has(s);
        }


        //----------------------------------------------------------------------------
        // Overloaded .Has methods that convert their argument into either a string
        // (property) or uint32 (index) and fowards the call to the correct .Has
        //----------------------------------------------------------------------------

        public bool Delete(BoxedValue index)
        {
            uint i = 0;
            if (TypeConverter.TryToIndex(index, ref i))
            {
                return this.Delete(i);
            }
            return this.Delete(TypeConverter.ToString(index));
        }

        public bool Delete(bool index)
        {
            return this.Delete(TypeConverter.ToString(index));
        }

        public bool Delete(double index)
        {
            uint parsed = 0;
            if (TypeConverter.TryToIndex(index, ref parsed))
            {
                return this.Delete(parsed);
            }
            return this.Delete(TypeConverter.ToString(index));
        }

        public bool Delete(object index)
        {
            string index = TypeConverter.ToString(index);
            uint parsed = 0;
            if (TypeConverter.TryToIndex(index, ref parsed))
            {
                return this.Delete(parsed);
            }
            return this.Delete(index);
        }

        public bool Delete(Undefined index)
        {
            return this.Has("undefined");
        }

        public bool Delete(CommonObject index)
        {
            string index = TypeConverter.ToString(index);
            uint parsed = 0;
            if (TypeConverter.TryToIndex(index, ref parsed))
            {
                return this.Delete(parsed);
            }
            return this.Delete(index);
        }

        public virtual Tuple<uint, HashSet<string>> CollectProperties()
        {
            return collectProperties(0, new HashSet<string>(), this);
        }

        private Tuple<uint, HashSet<string>> collectProperties(uint length, HashSet<string> set, CommonObject current)
        {
            if (current != null)
            {
                var array = current as ArrayObject;
                if (array != null)
                {
                    length = length < array.Length ? array.Length : length;
                }

                foreach (var pair in current.PropertySchema.IndexMap)
                {
                    var descriptor = current.Properties[pair.Value];
                    if (descriptor.HasValue && descriptor.IsEnumerable)
                    {
                        set.Add(pair.Key);
                    }
                }

                return collectProperties(length, set, current.Prototype);
            }
            else
            {
                return Tuple.Create(length, set);
            }
        }

        public virtual IEnumerable<BoxedValue> CollectIndexValues()
        {
            var length = this.Length;
            var index = 0u;
            while (index < length)
            {
                yield return this.Get(index);
                index++;
            }
        }
    }
}

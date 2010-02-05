using System;
using System.Collections.Generic;
using System.Reflection;
using IronJS.Runtime.Js.Descriptors;

using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Js
{
    public class JsArray : IObj
    {
        static public readonly ConstructorInfo Ctor
            = typeof(Obj).GetConstructor(Type.EmptyTypes);

        public LengthProperty Length { get; protected set; }
        public IDescriptor<IObj>[] Values { get; internal set; }
        public Dictionary<object, IDescriptor<IObj>> Properties { get; protected set; }

        public JsArray()
        {
            Length = new LengthProperty(this);
            Properties = new Dictionary<object, IDescriptor<IObj>>();
            Values = new IDescriptor<IObj>[0];
            Properties.Add("length", Length);
        }

        public override string ToString()
        {
            return (string) (this.Search("join") as IFunction).Call(this, new object[] { "," });
        }

        #region IObj Members

        public ObjClass Class { get; set; }
        public IObj Prototype { get; set; }
        public Context Context { get; set; }

        public bool Has(object name)
        {
            // Array
            if (name is int)
            {
                var asInt = (int)name;

                if (asInt >= Values.Length)
                    return false;

                return Values[asInt] != null;
            }

            // Property
            return Properties.ContainsKey(name);
        }

        public void Set(object name, IDescriptor<IObj> descriptor)
        {
            // Array
            if (name is int)
            {
                var asInt = (int)name;

                if (asInt >= Values.Length)
                {
                    var newArray = new IDescriptor<IObj>[asInt + 1];
                    Array.Copy(Values, newArray, Values.Length);
                    Values = newArray;
                }

                Values[asInt] = descriptor;

                return; // return void
            }

            // Property
            Properties.Add(name, descriptor);
        }

        public bool Get(object name, out IDescriptor<IObj> descriptor)
        {
            // Array
            if (name is int)
            {
                var asInt = (int)name;

                if (asInt >= Values.Length)
                {
                    descriptor = null;
                    return false;
                }

                if (Values[asInt] != null)
                {
                    descriptor = Values[asInt];
                    return true;
                }

                descriptor = null;
                return false;
            }

            // Property
            return Properties.TryGetValue(name, out descriptor);
        }

        public bool CanSet(object name)
        {
            // Array
            if (name is int)
            {
                var asInt = (int)name;
                
                if(asInt >= Values.Length)
                    return true;

                if (Values[asInt] != null)
                    return !Values[asInt].IsReadOnly;

                return true;
            }

            // Property
            IDescriptor<IObj> descriptor;
            return Properties.TryGetValue(name, out descriptor) ? !descriptor.IsReadOnly : true;
        }

        public bool TryDelete(object name)
        {
            // Array
            if (name is int)
            {
                var asInt = (int)name;

                if (asInt >= Values.Length)
                    return true;

                Values[asInt] = null;
            }

            // Property
            return Properties.Remove(name);
        }

        public object DefaultValue(ValueHint hint)
        {
            if (hint == ValueHint.String)
                return this.DefaultValueString();

            return this.DefaultValueNumber();
        }

        public List<KeyValuePair<object, IDescriptor<IObj>>> GetAllPropertyNames()
        {
            throw new NotImplementedException("ForIn for arrays are working atm");
        }

        #endregion

        #region IDynamicMetaObjectProvider Members

        public Meta GetMetaObject(Et parameter)
        {
            return new IObjMeta(parameter, this);
        }

        #endregion
    }
}

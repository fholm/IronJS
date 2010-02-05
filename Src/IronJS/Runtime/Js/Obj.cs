using System;
using System.Collections.Generic;

using System.Reflection;

using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Js
{
    public class Obj : IObj
    {
        static public readonly ConstructorInfo Ctor 
            = typeof(Obj).GetConstructor(Type.EmptyTypes);

        protected readonly Dictionary<object, IDescriptor<IObj>> Properties
            = new Dictionary<object, IDescriptor<IObj>>();

        #region IObj Members

        public ObjClass Class { get; set; }
        public IObj Prototype { get; set; }
        public Context Context { get; set; }

        public bool Has(object name)
        {
            return Properties.ContainsKey(name);
        }

        public void Set(object name, IDescriptor<IObj> descriptor)
        {
            Properties[name] = descriptor;
        }

        public bool Get(object name, out IDescriptor<IObj> descriptor)
        {
            return Properties.TryGetValue(name, out descriptor);
        }

        public bool CanSet(object name)
        {
            IDescriptor<IObj> descriptor;

            if (Properties.TryGetValue(name, out descriptor))
                return !descriptor.IsReadOnly;

            return true;
        }

        public bool TryDelete(object name)
        {
            IDescriptor<IObj> descriptor;

            if (Properties.TryGetValue(name, out descriptor))
                return descriptor.IsDeletable ? Properties.Remove(name) : false;

            return true; //TODO: Verify we should return true if property doesn't exist
        }

        public object DefaultValue(ValueHint hint)
        {
            if (hint == ValueHint.String)
                return this.DefaultValueString();

            return this.DefaultValueNumber();
        }

        public List<KeyValuePair<object, IDescriptor<IObj>>> GetAllPropertyNames()
        {
            return Properties.ToList();
        }

        #endregion

        #region IDynamicMetaObjectProvider Members

        public virtual Meta GetMetaObject(Et parameter)
        {
            return new IObjMeta(parameter, this);
        }

        #endregion
    }
}

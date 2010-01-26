using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Et = System.Linq.Expressions.Expression;
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
                return descriptor.IsReadOnly;

            return true;
        }

        public bool TryDelete(object name)
        {
            return Properties.Remove(name);
        }

        public object DefaultValue(ValueHint hint)
        {
            object toString;
            object valueOf;

            if (hint == ValueHint.String)
            {
                toString = this.Search("toString");
                if (toString is IFunction)
                    return (toString as IFunction).Call(this, null);

                valueOf = this.Search("valueOf");
                if (valueOf is IFunction)
                    return (valueOf as IFunction).Call(this, null);

                throw new ShouldThrowTypeError();
            }

            valueOf = this.Search("valueOf");
            if (valueOf is IFunction)
                return (valueOf as IFunction).Call(this, null);

            toString = this.Search("toString");
            if (toString is IFunction)
                return (toString as IFunction).Call(this, null);

            throw new ShouldThrowTypeError();
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

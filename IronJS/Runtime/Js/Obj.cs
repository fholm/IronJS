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

        protected readonly Dictionary<object, Property> Properties 
            = new Dictionary<object, Property>();

        public override string ToString()
        {
            return "IronJS: " + Class;
        }

        #region IObj Members

        public Context Context { get; set; }
        public ObjClass Class { get; set; }
        public IObj Prototype { get; set; }

        public virtual object DefaultValue(ValueHint hint)
        {
            object toString;
            object valueOf;

            if (hint == ValueHint.String)
            {
                toString = Get("toString");
                if (toString is IFunction)
                    return (toString as IFunction).Call(this, null);

                valueOf = Get("valueOf");
                if (valueOf is IFunction)
                    return (valueOf as IFunction).Call(this, null);

                throw new ShouldThrowTypeError();
            }

            valueOf = Get("valueOf");
            if (valueOf is IFunction)
                return (valueOf as IFunction).Call(this, null);

            toString = Get("toString");
            if (toString is IFunction)
                return (toString as IFunction).Call(this, null);

            throw new ShouldThrowTypeError();
        }

        public virtual object Get(object key)
        {
            Property result;

            if (Properties.TryGetValue(key, out result))
                return result.Value;

            if (Prototype != null)
                return Prototype.Get(key);

            return Js.Undefined.Instance;
        }

        public bool TryGet(object name, out object value)
        {
            Property result;

            if (Properties.TryGetValue(name, out result))
            {
                value = result.Value;
                return true;
            }

            if (Prototype != null)
                return Prototype.TryGet(name, out value);

            value = null;
            return false;
        }

        public virtual object Set(object key, object value)
        {
            IObj obj = this;

            while (obj != null)
            {
                if (obj.HasOwn(key))
                    return obj.SetOwn(key, value);

                obj = obj.Prototype;
            }

            Properties[key] = new Property(value);
            return value;
        }

        public virtual bool CanSet(object key)
        {
            if (Properties.ContainsKey(key))
                return Properties[key].NotHasAttr(PropertyAttrs.ReadOnly);

            if (Prototype != null)
                return Prototype.CanSet(key);

            return true;
        }

        public virtual bool HasOwn(object key)
        {
            return Properties.ContainsKey(key);
        }

        public virtual object GetOwn(object key)
        {
            Property property;

            if (Properties.TryGetValue(key, out property))
                return property.Value;

            return Js.Undefined.Instance;
        }

        public virtual object SetOwn(object key, object value)
        {
            Properties[key] = new Property(value);
            return value;
        }

        public virtual bool Has(object name)
        {
            if (HasOwn(name))
                return true;

            if (Prototype != null)
                return Prototype.Has(name);

            return false;
        }

        public virtual bool TryDelete(object name)
        {
            if (HasOwn(name))
                return Properties.Remove(name);

            return false;
        }

        public virtual List<object> GetAllPropertyNames()
        {
            return Properties.Keys.ToList();
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

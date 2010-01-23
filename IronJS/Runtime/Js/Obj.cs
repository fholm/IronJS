using System;
using System.Collections.Generic;
using System.Dynamic;
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

        public Obj()
        {

        }

        public override string ToString()
        {
            return "[object " + Class + "]";
        }

        #region IDynamicMetaObjectProvider Members

        Meta IDynamicMetaObjectProvider.GetMetaObject(Et parameter)
        {
            return new IObjMeta(parameter, this);
        }

        #endregion

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

        public virtual object Put(object key, object value)
        {
            IObj obj = this;

            while (obj != null)
            {
                if (obj.HasOwnProperty(key))
                    return obj.SetOwnProperty(key, value);

                obj = obj.Prototype;
            }

            Properties[key] = new Property(value);
            return value;
        }

        public virtual bool CanPut(object key)
        {
            if (Properties.ContainsKey(key))
                return Properties[key].NotHasAttr(PropertyAttrs.ReadOnly);

            if (Prototype != null)
                return Prototype.CanPut(key);

            return true;
        }

        public virtual bool HasOwnProperty(object key)
        {
            return Properties.ContainsKey(key);
        }

        public virtual object GetOwnProperty(object key)
        {
            Property property;

            if (Properties.TryGetValue(key, out property))
                return property.Value;

            return Js.Undefined.Instance;
        }

        public virtual object SetOwnProperty(object key, object value)
        {
            Properties[key] = new Property(value);
            return value;
        }

        public virtual bool HasProperty(object name)
        {
            if (HasOwnProperty(name))
                return true;

            if (Prototype != null)
                return Prototype.HasProperty(name);

            return false;
        }

        public virtual bool Delete(object name)
        {
            if (HasOwnProperty(name))
                return Properties.Remove(name);

            return false;
        }

        public virtual List<object> GetAllPropertyNames()
        {
            return Properties.Keys.ToList();
        }

        #endregion
    }
}

using System;
using System.Linq;
using System.Collections.Generic;
using System.Dynamic;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Js
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;

    public class Obj : IObj
    {
        protected readonly Dictionary<object, Property> Properties = 
                       new Dictionary<object, Property>();

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
            if (hint == ValueHint.Number)
                return 1.0;

            return "";
        }

        public virtual object Get(object key)
        {
            Property prop;

            if (Properties.TryGetValue(key.ToString(), out prop))
                return prop.Value;

            if (Prototype != null)
                return Prototype.Get(key);

            return Js.Undefined.Instance;
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
            return Properties.ContainsKey(key.ToString());
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

            throw new NotImplementedException("Deleting properties in Prototype not supported");
        }

        public virtual List<object> GetAllPropertyNames()
        {
            return Properties.Keys.ToList();
        }

        #endregion
    }
}

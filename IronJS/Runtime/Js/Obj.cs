using System;
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
        internal readonly Dictionary<object, Property> Properties =
            new Dictionary<object, Property>();

        public Obj()
        {

        }

        // 8.6.2
        public object DefaultValue(ValueHint hint)
        {
            if (hint == ValueHint.Number)
                return 1.0;

            return "";
        }

        // 8.6.2
        public object Get(object key)
        {
            Property prop;

            if (Properties.TryGetValue(key.ToString(), out prop))
                return prop.Value;

            if (Prototype != null)
                return Prototype.Get(key);

            return Js.Undefined.Instance;
        }

        // 8.6.2
        public object Put(object key, object value)
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

        // 8.6.2
        public bool CanPut(object key)
        {
            if (Properties.ContainsKey(key))
                return Properties[key].NotHasAttr(PropertyAttrs.ReadOnly);

            if (Prototype != null)
                return Prototype.CanPut(key);

            return true;
        }

        public bool HasOwnProperty(object key)
        {
            return Properties.ContainsKey(key.ToString());
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

        public object GetOwnProperty(object key)
        {
            Property property;

            if (Properties.TryGetValue(key, out property))
                return property.Value;

            return Js.Undefined.Instance;
        }

        public object SetOwnProperty(object key, object value)
        {
            Properties[key] = new Property(value);
            return value;
        }

        public bool HasProperty(object name)
        {
            if (HasOwnProperty(name))
                return true;

            if (Prototype != null)
                return Prototype.HasProperty(name);

            return false;
        }

        public bool Delete(object name)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

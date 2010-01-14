using System;
using System.Collections.Generic;
using System.Dynamic;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Js
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;

    public class Obj : IDynamicMetaObjectProvider, IObj
    {
        internal readonly Dictionary<object, Property> Properties =
            new Dictionary<object, Property>();

        // 8.6.2 'Scope'
        public readonly IFrame Frame;

        // 8.6.2 'Call'
        public readonly Lambda Lambda;

        public Obj()
        {
            Class = ObjClass.Object;
        }

        public Obj(IFrame frame, Lambda lambda)
        {
            Frame = frame;
            Lambda = lambda;
            Class = ObjClass.Function;
        }

        // 8.6.2
        /*
        public IObj Construct()
        {
            var newObject = new Obj();

            object prototype = GetOwnProperty("prototype");

            newObject.Prototype = (prototype is Obj)
                                ? (Obj) prototype
                                : GetObjectPrototype();

            return newObject;
        }
        */

        // 8.6.2
        public object DefaultValue(ValueHint hint)
        {
            return null;
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
                    return obj.SetOwnProperty(key);

                obj = obj.Prototype;
            }

            Properties[key] = new Property(value);
            return value;
        }

        public object PutWithAttrs(object key, object value, Js.PropertyAttrs attrs)
        {
            Obj obj = this;

            while (obj != null)
            {
                if (Properties.ContainsKey(key))
                    return Properties[key].Value = value;

                obj = (Obj) obj.Prototype;
            }

            Properties[key] = new Property(value, attrs);
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

        public bool SetIfExists(object key, object value)
        {
            Obj obj = this;

            while (obj != null)
            {
                if (Properties.ContainsKey(key))
                {
                    Properties[key].Value = value;
                    return true;
                }

                obj = (Obj) obj.Prototype;
            }

            return false;
        }

        public bool HasOwnProperty(object key)
        {
            return Properties.ContainsKey(key);
        }

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

        public override string ToString()
        {
            return "[object " + Class + "]";
        }

        #region IDynamicMetaObjectProvider Members

        Meta IDynamicMetaObjectProvider.GetMetaObject(Et parameter)
        {
            return new ObjMeta(parameter, this);
        }

        #endregion

        private IObj GetObjectPrototype()
        {
            IObj obj = this;

            while (obj.Prototype != null)
                obj = obj.Prototype;

            return obj;
        }

        #region IObj Members

        public bool HasProperty(object name)
        {
            throw new NotImplementedException();
        }

        public object SetOwnProperty(object name)
        {
            throw new NotImplementedException();
        }

        public bool Delete(object name)
        {
            throw new NotImplementedException();
        }

        public Context Context
        {
            get;
            set;
        }

        public ObjClass Class
        {
            get;
            set;
        }

        public IObj Prototype
        {
            get;
            set;
        }

        #endregion
    }
}

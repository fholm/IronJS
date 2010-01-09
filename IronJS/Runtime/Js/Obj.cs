using System;
using System.Collections.Generic;
using System.Dynamic;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Js
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;

    public enum ObjClass { Object, Function }

    public class Obj : IDynamicMetaObjectProvider
    {
        internal readonly Dictionary<object, Property> Properties =
            new Dictionary<object, Property>();

        // 8.6.2 'Scope'
        public readonly Frame Frame;

        // 8.6.2 'Call'
        public readonly Lambda Lambda;

        // 8.6.2
        public Obj Prototype;

        // 8.6.2
        public readonly ObjClass Class;

        // 8.6.2
        public object Value;

        public Obj()
        {
            Class = ObjClass.Object;
        }

        public Obj(Frame frame, Lambda lambda)
        {
            Frame = frame;
            Lambda = lambda;
            Class = ObjClass.Function;
        }

        // 8.6.2
        public Obj Construct()
        {
            var newObject = new Obj();

            object prototype = GetOwnProperty("prototype");

            newObject.Prototype = (prototype is Obj)
                                ? (Obj) prototype
                                : GetObjectPrototype();

            return newObject;
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
            Obj obj = this;

            while (obj != null)
            {
                if (Properties.ContainsKey(key))
                    return Properties[key].Value = value;

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

                obj = obj.Prototype;
            }

            Properties[key] = new Property(value, attrs);
            return value;
        }

        // 8.6.2.
        public bool CanPut(object key)
        {
            Obj obj = this;

            while (obj != null)
            {
                if (Properties.ContainsKey(key))
                    return Properties[key].NotHasAttr(PropertyAttrs.ReadOnly);

                obj = obj.Prototype;
            }

            return true;
        }
        
        // 8.6.2
        public bool Delete(object key)
        {
            return Properties.Remove(key);
        }

        public object HasOwnProperty(object key)
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

        private Obj GetObjectPrototype()
        {
            var obj = this;

            while (obj.Prototype != null)
                obj = obj.Prototype;

            return obj;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

namespace IronJS.Runtime.Js
{
    public class Obj : IDynamicMetaObjectProvider
    {
        internal readonly string Name;

        internal bool IsCallable { get { return Call != null; } }

        internal readonly Dictionary<object, Property> Properties =
            new Dictionary<object, Property>();

        // 8.6.2
        internal string Class;

        // 8.6.2
        /*
         * This is the internal
         * prototype of this object,
         * this is not the same as the
         * public property 'prototype'.
         */
        internal Obj Prototype;

        // 8.6.2
        /*
         * Scope isn't needed because
         * of how the DLR works, we only
         * need to create an implicit
         * this variable on all functions
         * that we can fill in with the
         * current context.
         */
        //public object Scope;

        // 8.6.2
        internal object Value;

        // 8.6.2
        public Delegate Call;

        public Obj(string name)
        {
            Name = name;
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

        public T GetAs<T>(object key)
        {
            return (T)Get(key);
        }

        // 8.6.2
        public object Put(object key, object value, Js.PropertyAttrs attrs)
        {
            Obj obj = this;

            while (obj != null)
            {
                if (Properties.ContainsKey(key))
                    return Properties[key].Value = value;

                obj = obj.Prototype;
            }

            Properties[key] = new Property(key.ToString(), value, attrs);
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
        public bool HasProperty(object key)
        {
            if (Properties.ContainsKey(key))
                return true;

            if (Prototype != null)
                return Prototype.HasProperty(key);

            return false;
        }

        // 8.6.2
        public bool Delete(object key)
        {
            return Properties.Remove(key);
        }

        // 8.6.2
        public object DefaultValue(object hint)
        {
            return null;
        }

        // 8.6.2
        public object Construct()
        {
            if (IsCallable)
            {
                var newObject = Context.CreateObject();
                object prototype = GetOwnProperty("prototype");

                newObject.Prototype = (prototype is Obj) 
                                      ? (Obj) prototype
                                      : Context.ObjectPrototype;

                return newObject;
            }

            throw new RuntimeError("Object isn't callable");
        }

        public object GetOwnProperty(object key)
        {
            Property prop;

            if (Properties.TryGetValue(key, out prop))
                return prop.Value;

            return Js.Undefined.Instance;
        }

        public override string ToString()
        {
            return "JsObject<" + Name + ">";
        }

        #region IDynamicMetaObjectProvider Members

        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Expression parameter)
        {
            return new ObjMeta(parameter, this);
        }

        #endregion
    }
}

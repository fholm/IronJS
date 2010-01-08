using System;
using System.Collections.Generic;
using IronJS.Runtime.Binders;

namespace IronJS.Runtime.Js
{
    using System.Dynamic;
    using Et = System.Linq.Expressions.Expression;

    public class Frame<T> : IDynamicMetaObjectProvider
    {
        readonly Frame<T> _parent;

        readonly Dictionary<object, T> _values =
             new Dictionary<object, T>();

        public Frame(Frame<T> parent)
        {
            _parent = parent;
        }

        public T Push(object key, T value)
        {
            return _values[key] = value;
        }

        public T Pull(object key)
        {
            if (_values.ContainsKey(key))
                return _values[key];

            if (_parent != null)
                return _parent.Pull(key);

            throw new MissingFieldException("No variable named " + key + " exists");
        }

        public Frame<T> Create()
        {
            return new Frame<T>(this);
        }

        #region IDynamicMetaObjectProvider Members

        DynamicMetaObject IDynamicMetaObjectProvider.GetMetaObject(Et parameter)
        {
            return new FrameMeta<T>(parameter, this);
        }

        #endregion

        internal static Et Var(Et scope, string name, Et value)
        {
            return Et.Dynamic(
                new JsSetMemberBinder(name),
                typeof(object),
                scope,
                value
            );
        }

        internal static Et Var(Et scope, string name)
        {
            return Et.Dynamic(
                new JsGetMemberBinder(name),
                typeof(object),
                scope
            );
        }

        internal static Et Var<Y>(Et scope, string name)
        {
            return Et.Convert(
                Et.Dynamic(
                    new JsGetMemberBinder(name),
                    typeof(object),
                    scope
                ),
                typeof(Y)
            );
        }

        internal static Et Create(Et target, Et parent)
        {
            return Et.Assign(
                target,
                Et.Call(
                    parent, 
                    typeof(Frame<T>).GetMethod(
                        "Create", 
                        Type.EmptyTypes
                    )
                )
            );
        }
    }
}

using System;
using System.Collections.Generic;
using IronJS.Runtime.Binders;

namespace IronJS.Runtime.Js
{
    using System.Dynamic;
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;

    public enum VarType { Global, Local }

    public class Frame : IDynamicMetaObjectProvider
    {
        readonly Frame _parent;

        readonly Dictionary<object, object> _values =
             new Dictionary<object, object>();

        public Frame(Frame parent)
        {
            _parent = parent;
        }

        public object Push(object key, object value, VarType type)
        {
            if (type == VarType.Local)
            {
                _values[key] = value;
            }
            else
            {
                if (_values.ContainsKey(key))
                {
                    _values[key] = value;
                }
                else
                {
                    if (_parent == null)
                    {
                        _values[key] = value;
                    }
                    else
                    {
                        return _parent.Push(key, value, VarType.Global);
                    }
                }
            }

            return value;
        }

        public object Pull(object key)
        {
            object value; 

            if (_values.objectryGetValue(key, out value))
                return value;

            if (_parent != null)
                return _parent.Pull(key);

            return Js.Undefined.Instance;
        }

        public Frame Enter()
        {
            return new Frame(this);
        }

        #region IDynamicMetaObjectProvider Members

        Meta IDynamicMetaObjectProvider.GetMetaObject(Et parameter)
        {
            return new FrameMeta<object>(parameter, this);
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

        internal static Et Enter(Et target, Et parent)
        {
            return Et.Assign(
                target,
                Et.Call(
                    parent, 
                    typeof(Frame).GetMethod(
                        "Enter", 
                        objectype.Emptyobjectypes
                    )
                )
            );
        }
    }
}

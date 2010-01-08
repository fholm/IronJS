using System.Collections.Generic;
using System.Dynamic;
using IronJS.Reflect;

namespace IronJS.Runtime.Js
{
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
            return new FrameMeta(parameter, this);
        }

        #endregion

        #region Static methods

        internal static Et Var(Et frame, string name, Et value)
        {
            return Et.Call(
                frame,
                Method.GetMethod<Frame>("Push"),
                value
            );
        }

        internal static Et Var(Et frame, string name)
        {
            return Et.Call(
                frame,
                Method.GetMethod<Frame>("Pull"),
                name
            );
        }

        internal static Et Var<T>(Et frame, string name)
        {
            return Et.Convert(
                Frame.Var(
                    frame,
                    name
                )
            );
        }

        internal static Et Enter(Et target, Et parent)
        {
            return Et.Assign(
                target,
                Et.Call(
                    parent, 
                    Method.GetMethod<Frame>("Enter")
                )
            );
        }

        #endregion
    }
}

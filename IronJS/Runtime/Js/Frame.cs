using System.Collections.Generic;
using System.Dynamic;
using IronJS.Reflect;
using System.Reflection;

namespace IronJS.Runtime.Js
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;

    public enum VarType { Global, Local }

    public class Frame
    {
        readonly Frame _parent;

        readonly Dictionary<object, object> _values =
             new Dictionary<object, object>();

        public Frame()
            : this(null)
        {

        }

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

            if (_values.TryGetValue(key, out value))
                return value;

            if (_parent != null)
                return _parent.Pull(key);

            return Js.Undefined.Instance;
        }

        public Frame Enter()
        {
            return new Frame(this);
        }

        #region Static

        internal static Et Var(Et frame, string name, Et value, VarType type)
        {
            return Et.Call(
                frame,
                typeof(Frame).GetMethod("Push"),
                Et.Constant(name),
                value,
                Et.Constant(type)
            );
        }

        internal static Et Var(Et frame, string name)
        {
            return Et.Call(
                frame,
                typeof(Frame).GetMethod("Pull"),
                Et.Constant(name)
            );
        }

        internal static Et Var<T>(Et frame, string name)
        {
            return Et.Convert(
                Frame.Var(
                    frame,
                    name
                ),
                typeof(T)
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

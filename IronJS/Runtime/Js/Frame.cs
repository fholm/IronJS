using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace IronJS.Runtime.Js
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;

    public enum VarType { Global, Local }
    public enum GetType { Value, Call }

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

        public object Arg(object key)
        {
            object value;

            if (_values.TryGetValue(key, out value))
                return value;

            return null;
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

        public object Pull(object key, GetType type)
        {
            object value; 

            if (_values.TryGetValue(key, out value))
                return value;

            if (_parent != null)
                return _parent.Pull(key, type);

            return Js.Undefined.Instance;
        }

        public Frame Enter()
        {
            return new Frame(this);
        }

        public Frame Exit()
        {
            return _parent;
        }

        #region Static

        #endregion
    }
}

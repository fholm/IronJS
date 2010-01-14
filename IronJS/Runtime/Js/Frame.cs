using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace IronJS.Runtime.Js
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;

    public enum VarType { Global, Local }
    public enum GetType { Value, Name }

    public class Frame : IFrame
    {
        readonly bool _isGlobal;
        readonly IFrame _parent;

        readonly Dictionary<object, object> _values =
             new Dictionary<object, object>();

        public Frame()
            : this(null)
        {

        }

        public Frame(IFrame parent, bool isGlobal)
            : this(parent)
        {
            _isGlobal = true;
        }


        public Frame(IFrame parent)
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

        #region IFrame Members

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
                    if (_isGlobal || _parent == null)
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

        public IFrame Enter()
        {
            return new Frame(this);
        }

        public IFrame Exit()
        {
            return _parent;
        }

        #endregion

        #region Static

        #endregion
    }
}

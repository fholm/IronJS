using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace IronJS.Runtime.Js
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;

    class WithFrame : IFrame
    {
        Obj _obj;
        IFrame _parent;

        public WithFrame(Obj obj, IFrame parent)
        {
            _obj = obj;
            _parent = parent;
        }

        #region IFrame Members

        public IFrame Enter()
        {
            return new Frame(this);
        }

        public IFrame Exit()
        {
            return _parent;
        }

        public object Pull(object key, GetType type)
        {
            var value = _obj.Get(key); // <- this is reason 1 with() {} is slow, 
                                       // so don't frekin use it

            if (value is Js.Undefined)
                return _parent.Pull(key, type);

            if (type == Js.GetType.Call)
                return new CallProxy(_obj, key);

            return value;
        }

        public object Push(object key, object value, VarType type)
        {
            if (_obj.SetIfExists(key, value)) // <- this is reason 2 with() {} is slow, 
                return value;                 // so don't frekin use it

            return _parent.Push(key, value, type);
        }

        #endregion
    }

    class CallProxy // <- this is reason 3 with() {} is slow, 
                    // so don't frekin use it
    {
        public readonly Obj That;
        public readonly object Method;

        public CallProxy(Obj that, object method)
        {
            That = that;
            Method = method;
        }
    }
}

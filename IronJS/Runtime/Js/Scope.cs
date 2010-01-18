using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Runtime.Js
{
    public class Scope : IScope
    {
        IObj _values;
        IScope _parent;
        Context _context;

        public Scope(Context context, IScope parent, IObj values)
        {
            _parent = parent;
            _values = values;
            _context = context;
        }

        public Scope(Context context, IScope parent)
            : this(context)
        {
            _parent = parent;
        }

        public Scope(Context context)
        {
            _context = context;
            _values = context.CreateObject();
            (_values as Obj).Prototype = null;
        }

        #region IScope Members

        public IScope Enter()
        {
            return new Scope(_context, this);
        }

        public IScope Enter(IObj obj)
        {
            return new Scope(_context, this, obj);
        }

        public IScope Exit()
        {
            return _parent;
        }

        public object Local(object name, object value)
        {
            return _values.Put(name, value);
        }

        public object Global(object name, object value)
        {
            if (_parent == null || _values.HasProperty(name))
                return _values.Put(name, value);

            return _parent.Global(name, value);
        }

        public object Pull(object name)
        {
            var value = _values.Get(name);

            if (value is Undefined && _parent != null)
                _parent.Pull(name);

            return value;
        }

        #endregion

        #region Static

        public static Scope CreateGlobal(Context context)
        {
            return new Scope(context);
        }

        #endregion
    }
}

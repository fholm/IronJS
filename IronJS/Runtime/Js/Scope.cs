using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Runtime.Js
{
    public class Scope
    {
        IObj _obj;
        Scope _parent;
        Context _context;

        public Scope(Context context, Scope parent, IObj values)
        {
            _parent = parent;
            _obj = values;
            _context = context;
        }

        public Scope(Context context, Scope parent)
            : this(context)
        {
            _parent = parent;
        }

        public Scope(Context context)
        {
            _context = context;
            _obj = context.CreateObject();
            (_obj as Obj).Class = ObjClass.Scope;
            (_obj as Obj).Prototype = null;
        }

        #region IScope Members

        public Scope Exit()
        {
            return _parent;
        }

        public object Local(object name, object value)
        {
            if (_obj.Class == ObjClass.Scope || _obj.HasProperty(name))
                return _obj.Put(name, value);

            return _parent.Local(name, value);
        }

        public object Global(object name, object value)
        {
            if (_parent == null || _obj.HasProperty(name))
                return _obj.Put(name, value);

            return _parent.Global(name, value);
        }

        public object Pull(object name)
        {
            var value = _obj.Get(name);

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

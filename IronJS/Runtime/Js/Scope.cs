using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace IronJS.Runtime.Js
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = System.Linq.Expressions.Expression;
    using EtParam = System.Linq.Expressions.ParameterExpression;
    using IronJS.Runtime.Utils;

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

        #region Expression Tree

        internal static Et EtLocal(EtParam scope, string name, Et value)
        {
            return Et.Call(
                scope,
                typeof(Scope).GetMethod("Local"),
                Et.Constant(name, typeof(object)),
                value
            );
        }

        internal static Et EtGlobal(EtParam scope, string name, Et value)
        {
            return Et.Call(
                scope,
                typeof(Scope).GetMethod("Global"),
                Et.Constant(name, typeof(object)),
                value
            );
        }

        internal static Et EtPull(EtParam scope, string name)
        {
            return Et.Call(
                scope,
                typeof(Scope).GetMethod("Pull"),
                Et.Constant(name, typeof(object))
            );
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

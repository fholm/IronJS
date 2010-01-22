using System;
using System.Dynamic;
using System.Reflection;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Et = System.Linq.Expressions.Expression;
using EtParam = System.Linq.Expressions.ParameterExpression;

namespace IronJS.Runtime.Js
{
    public class Scope
    {
        static public readonly ConstructorInfo Ctor1Args
            = typeof(Scope).GetConstructor(new[] { typeof(Scope) });

        static public readonly ConstructorInfo Cto2Args
            = typeof(Scope).GetConstructor(new[] { typeof(Scope), typeof(IObj) });

        static public readonly PropertyInfo PiParent
            = typeof(Scope).GetProperty("Parent");

        IObj _obj;
        Scope _parent;
        bool _privateObj = false;

        public IObj Value { get { return _obj; } }
        public Scope Parent { get { return _parent; } }
        public Scope Globals { get { return _parent == null ? this : _parent.Globals; } }

        public Scope(Scope parent, IObj obj)
        {
            _obj = obj;
            _parent = parent;
        }

        public Scope(Context context, Scope parent)
            : this(context)
        {
            _parent = parent;
        }

        public Scope(Context context)
        {
            _privateObj = true;
            _obj = context.CreateObject();
            (_obj as Obj).Prototype = null;
        }

        public object Call(object name, object[] args)
        {
            if (_obj.HasProperty(name))
            {
                var obj = _obj.Get(name);

                if (obj is IFunction)
                {
                    if (_privateObj)
                        return (obj as IFunction).Call(Globals.Value, args);

                    return (obj as IFunction).Call(Value, args);
                }

                if (obj is MethodInfo)
                {
                    var mi = obj as MethodInfo;
                    return mi.Invoke(null, args);
                }

                if (obj is Delegate)
                {
                    var dg = obj as Delegate;
                    var invoke = dg.GetType().GetMethod("Invoke");
                    return invoke.Invoke(dg, args);
                }

                throw new InternalRuntimeError("Can't call non function: '" + name + "'");
            }

            if (_parent == null)
                throw new InternalRuntimeError("Variable '" + name + "' is not defined");

            return _parent.Call(name, args);
        }

        public object Delete(object name)
        {
            if(_obj.Delete(name))
                return true;

            if (_parent != null)
                return _parent.Delete(name);

            return false;
        }

        public object Local(object name, object value)
        {
            if (_privateObj || _obj.HasProperty(name))
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
            //TODO: implement obj.TryGet(name, out value) maybe, so we don't have to check for is Undefined.
            var value = _obj.Get(name);

            if (value is Undefined)
            {
                if(_parent != null)
                    return _parent.Pull(name);

                if(name is string && (string)name != "undefined")
                    throw new InternalRuntimeError("Variable '" + name + "' is not defined");
            }

            return value;
        }

        public Scope Enter()
        {
            return new Scope(_obj.Context, this);
        }

        #region Expression Tree

        internal static Et EtDelete(EtParam scope, string name)
        {
            return Et.Call(
                scope,
                typeof(Scope).GetMethod("Delete"),
                Et.Constant(name)
            );
        }

        internal static Et EtValue(EtParam scope)
        {
            return Et.Property(
                scope,
                "Value"
            );
        }

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

        internal static Et EtNew(Et context, Et parent)
        {
            return AstUtils.SimpleNewHelper(
                Ctor1Args,
                context,
                parent
            );
        }

        internal static Et EtNewPrivate(Et parent, Et obj)
        {
            return AstUtils.SimpleNewHelper(
                Cto2Args,
                parent,
                obj
            );
        }

        internal static Et EtExit(Et scope)
        {
            return Et.Property(
                scope,
                PiParent
            );
        }

        #endregion

        #region Static

        public static Scope CreateGlobal(Context context)
        {
            return new Scope(context);
        }

        public static Scope CreateCallScope(Scope closure, IFunction callee, IObj that, object[] args)
        {
            return CreateCallScope(closure, callee, that, args, new string[] { }); // TODO: not necessary to create a new array here each time
        }

        public static Scope CreateCallScope(Scope closure, IFunction callee, IObj that, object[] args, string[] parms)
        {
            var callScope = closure.Enter();
            var argsObject = closure._obj.Context.ObjectConstructor.Construct();

            callScope.Local("this", that);
            callScope.Local("arguments", argsObject);
            argsObject.SetOwnProperty("length", args.Length);
            argsObject.SetOwnProperty("callee", callee);

            for (var i = 0; i < args.Length; ++i)
            {
                if (i < parms.Length)
                    callScope.Local(parms[i], args[i]);

                argsObject.SetOwnProperty((double)i, args[i]);
            }

            return callScope;
        }

        #endregion
    }
}

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

        static public readonly ConstructorInfo Ctor2Args
            = typeof(Scope).GetConstructor(new[] { typeof(Scope), typeof(IObj) });

        static public readonly PropertyInfo PiParentScope
            = typeof(Scope).GetProperty("ParentScope");

        static public readonly PropertyInfo PiJsObject
            = typeof(Scope).GetProperty("JsObject");

        bool IsInternal { get { return JsObject.Class == ObjClass.Internal; } }

        public IObj JsObject { get; protected set; }
        public Scope ParentScope { get; protected set; }
        public Scope Globals { get { return ParentScope == null ? this : ParentScope.Globals; } }

        public Scope(Scope parentScope, IObj jsObject)
        {
            JsObject = jsObject;
            ParentScope = parentScope;
        }

        public Scope(Context context, Scope parentScope)
            : this(context)
        {
            ParentScope = parentScope;
        }

        public Scope(Context context)
        {
            JsObject = context.CreateObject();
            JsObject.Prototype = null;
            JsObject.Class = ObjClass.Internal;
        }

        /// <summary>
        /// This function in combination with the special with-statement handling code
        /// in CallNode.Walk() handles function calls that are inside with-statements
        /// </summary>
        /// <param name="name">Function name to call</param>
        /// <param name="args">Arguments to function</param>
        /// <returns>Function result</returns>
        public object Call(object name, object[] args)
        {
            if (JsObject.HasProperty(name))
            {
                var obj = JsObject.Get(name);

                if (obj is IFunction)
                {
                    if (IsInternal)
                        return (obj as IFunction).Call(Globals.JsObject, args);

                    return (obj as IFunction).Call(JsObject, args);
                }

                if (obj is MethodInfo)
                    return (obj as MethodInfo).Invoke(null, args);

                if (obj is Delegate)
                    return (obj as Delegate).DynamicInvoke(args);

                throw InternalRuntimeError.New(
                    InternalRuntimeError.NOT_CALLABLE, 
                    name
                );
            }

            if (ParentScope == null)
                throw InternalRuntimeError.New(
                    InternalRuntimeError.NOT_DEFINED,
                    name
                );

            return ParentScope.Call(name, args);
        }

        public object Delete(object name)
        {
            if(JsObject.Delete(name))
                return true;

            if (ParentScope != null)
                return ParentScope.Delete(name);

            return false;
        }

        public object Local(object name, object value)
        {
            if (IsInternal || JsObject.HasProperty(name))
                return JsObject.Put(name, value);

            return ParentScope.Local(name, value);
        }

        public object Global(object name, object value)
        {
            if (ParentScope == null || JsObject.HasProperty(name))
                return JsObject.Put(name, value);

            return ParentScope.Global(name, value);
        }

        public object Pull(object name)
        {
            //TODO: implement obj.TryGet(name, out value) maybe, so we don't have to check for is Undefined.
            var value = JsObject.Get(name);

            if (value is Undefined)
            {
                if(ParentScope != null)
                    return ParentScope.Pull(name);

                if(name is string && (string)name != "undefined")
                    throw InternalRuntimeError.New(
                        InternalRuntimeError.NOT_DEFINED,
                        name
                    );
            }

            return value;
        }

        public Scope Enter()
        {
            return new Scope(JsObject.Context, this);
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

        internal static Et EtJsObject(EtParam scope)
        {
            return Et.Property(
                scope,
                PiJsObject
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
                Ctor2Args,
                parent,
                obj
            );
        }

        internal static Et EtExit(Et scope)
        {
            return Et.Property(
                scope,
                PiParentScope
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
            var argsObject = closure.JsObject.Context.ObjectConstructor.Construct();

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

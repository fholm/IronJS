using System;
using System.Dynamic;
using System.Reflection;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Et = System.Linq.Expressions.Expression;
using EtParam = System.Linq.Expressions.ParameterExpression;

namespace IronJS.Runtime.Js
{
    sealed public class Scope
    {
        static public readonly ConstructorInfo Ctor1Args
            = typeof(Scope).GetConstructor(new[] { typeof(Scope) });

        static public readonly ConstructorInfo Ctor2Args
            = typeof(Scope).GetConstructor(new[] { typeof(Scope), typeof(IObj) });

        static public readonly PropertyInfo PiParentScope
            = typeof(Scope).GetProperty("ParentScope");

        static public readonly PropertyInfo PiJsObject
            = typeof(Scope).GetProperty("JsObject");

        static public readonly MethodInfo MiCall
            = typeof(Scope).GetMethod("Call");

        static public readonly MethodInfo MiDelete
            = typeof(Scope).GetMethod("Delete");

        static public readonly MethodInfo MiLocal
            = typeof(Scope).GetMethod("Local");

        static public readonly MethodInfo MiGlobal
            = typeof(Scope).GetMethod("Global");

        static public readonly MethodInfo MiPull
            = typeof(Scope).GetMethod("Pull");

        bool IsInternal { get { return JsObject.Class == ObjClass.Internal; } }

        public IObj JsObject { get; private set; }
        public Scope ParentScope { get; private set; }
        public Scope Globals { get { return (ParentScope == null ? this : ParentScope.Globals); } }

        public Scope(Scope parentScope, IObj jsObject)
        {
            JsObject = jsObject;
            ParentScope = parentScope;
        }

        public Scope(Context context, Scope parentScope)
        {
            ParentScope = parentScope;

            JsObject = context.CreateObject();
            JsObject.Prototype = null;
            JsObject.Class = ObjClass.Internal;
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
            object value;

            if(!JsObject.TryGet(name, out value))
            {
                if(ParentScope != null)
                    return ParentScope.Pull(name);

                throw InternalRuntimeError.New(
                    InternalRuntimeError.NOT_DEFINED,
                    name
                );
            }

            return value;
        }

        public object Delete(object name)
        {
            if (JsObject.Delete(name))
                return true;

            if (ParentScope != null)
                return ParentScope.Delete(name);

            return false;
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
            object obj;

            if (JsObject.TryGet(name, out obj))
            {
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

        #region Static

        public static Scope CreateGlobal(Context context)
        {
            return new Scope(context, null);
        }

        public static Scope CreateCallScope(Scope closure, IFunction callee, IObj that, object[] args)
        {
            return CreateCallScope(closure, callee, that, args, null);
        }

        public static Scope CreateCallScope(Scope closure, IFunction callee, IObj that, object[] args, string[] parms)
        {
            var callScope = new Scope(closure.JsObject.Context, closure);
            var argsObject = closure.JsObject.Context.ObjectConstructor.Construct();

            callScope.Local("this", that);
            callScope.Local("arguments", argsObject);

            argsObject.SetOwnProperty("length", args.Length);
            argsObject.SetOwnProperty("callee", callee);

            for (var i = 0; i < args.Length; ++i)
            {
                if (parms != null && i < parms.Length)
                    callScope.Local(parms[i], args[i]);

                argsObject.SetOwnProperty((double)i, args[i]);
            }

            return callScope;
        }

        #endregion
    }
}

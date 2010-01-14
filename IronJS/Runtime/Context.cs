using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Js;
using IronJS.Runtime.Binders;
using System.Linq.Expressions;
using System.Dynamic;

namespace IronJS.Runtime
{
    public class Context
    {
        public IFrame SuperGlobals { get; protected set; }

        public Obj Object { get; protected set; }
        public Obj ObjectPrototype { get; protected set; }
        public Obj Function { get; protected set; }
        public Obj FunctionPrototype { get; protected set; }

        protected Context()
        {

        }

        public IFrame Run(Action<IFrame> delegat)
        {
            var globals = new Frame(SuperGlobals, true);

            delegat(globals);

            return globals;
        }

        public IObj CreateObject()
        {
            var obj = new Obj();

            obj.Context = this;
            obj.Class = ObjClass.Object;
            obj.Prototype = ObjectPrototype;

            return obj;
        }

        public IObj CreateFunction(IFrame frame, Lambda lambda)
        {
            var obj = new Function(frame, lambda);

            obj.Context = this;
            obj.Class = ObjClass.Function;
            obj.Prototype = FunctionPrototype;

            return obj;
        }

        #region binders

        internal JsBinaryOpBinder CreateBinaryOpBinder(ExpressionType op)
        {
            return new JsBinaryOpBinder(op, this);
        }

        internal JsUnaryOpBinder CreateUnaryOpBinder(ExpressionType op)
        {
            return new JsUnaryOpBinder(op, this);
        }

        internal JsConvertBinder CreateConvertBinder(Type type)
        {
            return new JsConvertBinder(type, this);
        }

        internal JsGetIndexBinder CreateGetIndexBinder(CallInfo callInfo)
        {
            return new JsGetIndexBinder(callInfo, this);
        }

        internal JsSetIndexBinder CreateSetIndexBinder(CallInfo callInfo)
        {
            return new JsSetIndexBinder(callInfo, this);
        }

        internal JsGetMemberBinder CreateGetMemberBinder(object name)
        {
            return new JsGetMemberBinder(name, this);
        }

        internal JsSetMemberBinder CreateSetMemberBinder(object name)
        {
            return new JsSetMemberBinder(name, this);
        }

        internal JsInvokeBinder CreateInvokeBinder(CallInfo callInfo, InvokeFlag flag)
        {
            return new JsInvokeBinder(callInfo, flag, this);
        }

        internal JsInvokeMemberBinder CreateInvokeMemberBinder(object name, CallInfo callInfo)
        {
            return new JsInvokeMemberBinder(name, callInfo, this);
        }

        #endregion

        static public Context Setup()
        {
            var ctx = new Context();

            ctx.SuperGlobals = new Frame();

            ctx.ObjectPrototype = new Obj();
            ctx.FunctionPrototype = new Obj(
                ctx.SuperGlobals,
                new Lambda(
                    new Func<IFrame, object>(FunctionPrototypeLambda),
                    new string[] { }.ToList()
                )
            );

            ctx.Object = new Obj(
                ctx.SuperGlobals,
                new Lambda(
                    new Func<IFrame, object>(ObjectConstructorLambda),
                    new[] { "value" }.ToList()
                )
            );

            ctx.Function = new Obj(
                ctx.SuperGlobals,
                new Lambda(
                    new Func<IFrame, object>(FunctionConstructorLambda),
                    new string[] { }.ToList()
                )
            );

            // Object
            ctx.Object.Prototype = ctx.FunctionPrototype;
            ctx.Object.SetOwnProperty("prototype", ctx.ObjectPrototype);

            // Function
            ctx.Function.Prototype = ctx.FunctionPrototype;
            ctx.Function.SetOwnProperty("prototype", ctx.FunctionPrototype);

            // Function.prototype
            ctx.FunctionPrototype.Prototype = ctx.ObjectPrototype;
            ctx.FunctionPrototype.SetOwnProperty("constructor", ctx.Function);

            // Push on global frame
            ctx.SuperGlobals.Push("Object", ctx.Object, VarType.Global);
            ctx.SuperGlobals.Push("Function", ctx.Function, VarType.Global);
            ctx.SuperGlobals.Push("undefined", Js.Undefined.Instance, VarType.Global);
            ctx.SuperGlobals.Push("Infinity", double.PositiveInfinity, VarType.Global);
            ctx.SuperGlobals.Push("NaN", double.NaN, VarType.Global);

            return ctx;
        }

        static public object FunctionPrototypeLambda(IFrame frame)
        {
            return Js.Undefined.Instance;
        }

        static public object FunctionConstructorLambda(IFrame frame)
        {
            return null;
        }

        static public object ObjectConstructorLambda(IFrame frame)
        {
            var value = frame.Arg("value");

            if (value != null || value == Js.Undefined.Instance)
            {
                throw new NotImplementedException("ToObject() not implemented");
            }

            return null;
        }
    }
}

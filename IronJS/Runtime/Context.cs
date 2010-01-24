using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using IronJS.Runtime.Binders;
using IronJS.Runtime.Builtins;
using IronJS.Runtime.Js;

// Aliases
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Runtime
{

    public class Context
    {
        public Object_ctor ObjectConstructor { get; protected set; }
        public Function_ctor FunctionConstructor { get; protected set; }
        public Array_ctor ArrayConstructor { get; protected set; }
        public String_ctor StringConstructor { get; protected set; }
        public Boolean_ctor BooleanConstructor { get; protected set; }
        public Number_ctor NumberConstructor { get; protected set; }
        public Math_obj MathObject { get; protected set; }

        protected Context()
        {
            FunctionConstructor = Function_ctor.Create(this);
            ObjectConstructor = Object_ctor.Create(this);
            ArrayConstructor = Array_ctor.Create(this);
            StringConstructor = String_ctor.Create(this);
            BooleanConstructor = Boolean_ctor.Create(this);
            NumberConstructor = Number_ctor.Create(this);
            MathObject = Math_obj.Create(this);

            ObjectConstructor.Prototype = FunctionConstructor.Function_prototype;

            FunctionConstructor.Prototype = FunctionConstructor.Function_prototype;
            FunctionConstructor.Function_prototype.Prototype = ObjectConstructor.Object_prototype;
        }

        public void SetupGlobals(Scope globals)
        {
            globals.Global("Object", ObjectConstructor);
            globals.Global("Function", FunctionConstructor);
            globals.Global("Array", ArrayConstructor);
            globals.Global("String", StringConstructor);
            globals.Global("Boolean", BooleanConstructor);
            globals.Global("Number", NumberConstructor);
            globals.Global("Math", MathObject);
            globals.Global("undefined", Js.Undefined.Instance);
            globals.Global("Infinity", double.PositiveInfinity);
            globals.Global("NaN", double.NaN);
            globals.Global("globals", globals.JsObject);
        }

        #region Object creators

        public Obj CreateObject()
        {
            var obj = new Obj();

            obj.Context = this;
            obj.Class = ObjClass.Object;

            return obj;
        }

        public IObj CreateNumber(object value)
        {
            return NumberConstructor.Construct(new[] { value });
        }

        public IObj CreateString(object value)
        {
            return StringConstructor.Construct(new[] { value });
        }

        public IObj CreateBoolean(object value)
        {
            return BooleanConstructor.Construct(new[] { value });
        }

        public Function CreateFunction(Scope scope, Lambda lambda)
        {
            var obj = new Function(scope, lambda);

            var protoObj = ObjectConstructor.Construct();
            protoObj.SetOwnProperty("constructor", obj);

            obj.Context = this;
            obj.Class = ObjClass.Function;
            obj.Prototype = FunctionConstructor.Function_prototype;
            obj.SetOwnProperty("prototype", protoObj);

            return obj;
        }

        #endregion

        #region Binders

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

        internal JsDeleteIndexBinder CreateDeleteIndexBinder(CallInfo callInfo)
        {
            return new JsDeleteIndexBinder(callInfo, this);
        }

        internal JsGetMemberBinder CreateGetMemberBinder(object name)
        {
            return new JsGetMemberBinder(name, this);
        }

        internal JsSetMemberBinder CreateSetMemberBinder(object name)
        {
            return new JsSetMemberBinder(name, this);
        }

        internal JsDeleteMemberBinder CreateDeleteMemberBinder(object name)
        {
            return new JsDeleteMemberBinder(name, this);
        }

        internal JsInvokeBinder CreateInvokeBinder(CallInfo callInfo)
        {
            return new JsInvokeBinder(callInfo, this);
        }

        internal JsInvokeMemberBinder CreateInvokeMemberBinder(object name, CallInfo callInfo)
        {
            return new JsInvokeMemberBinder(name, callInfo, this);
        }

        internal JsCreateInstanceBinder CreateInstanceBinder(CallInfo callInfo)
        {
            return new JsCreateInstanceBinder(callInfo, this);
        }

        #endregion

        #region Static

        static public Context Setup()
        {
            var ctx = new Context();

            /*
            // Object
            (ctx.ObjectConstructor as Function).Prototype = ctx.FunctionPrototype;
            ctx.ObjectConstructor.SetOwnProperty("prototype", ctx.ObjectPrototype);

            // Function
            (ctx.FunctionConstructor as Function).Prototype = ctx.FunctionPrototype;
            ctx.FunctionConstructor.SetOwnProperty("prototype", ctx.FunctionPrototype);

            // Function.prototype
            (ctx.FunctionPrototype as Function).Prototype = ctx.ObjectPrototype;
            ctx.FunctionPrototype.SetOwnProperty("constructor", ctx.FunctionConstructor);
            */

            return ctx;
        }

        #region Expression Tree

        static internal Et EtCreateFunction(Context context, Et scope, Et lambda)
        {
            return Et.Call(
                Et.Constant(context),
                typeof(Context).GetMethod("CreateFunction"),
                scope,
                lambda
            );
        }

        #endregion

        #endregion

        #region Methods

        static public class Methods
        {
            static public MethodInfo CreateFunction = typeof(Context).GetMethod("CreateFunction", new[] { typeof(IObj), typeof(Lambda) });
            static public MethodInfo CreateObjectCtor = typeof(Context).GetMethod("CreateObject", new[] { typeof(IObj) });
            static public MethodInfo CreateObject = typeof(Context).GetMethod("CreateObject", Type.EmptyTypes);
            static public MethodInfo CreateArray = typeof(Context).GetMethod("CreateArray", Type.EmptyTypes);
            static public MethodInfo CreateString = typeof(Context).GetMethod("CreateString", new[] { typeof(object) });
            static public MethodInfo CreateNumber = typeof(Context).GetMethod("CreateNumber", new[] { typeof(object) });
            static public MethodInfo CreateBoolean = typeof(Context).GetMethod("CreateBoolean", new[] { typeof(object) });
        }

        #endregion
    }
}

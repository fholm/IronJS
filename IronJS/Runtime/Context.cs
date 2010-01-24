using System;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using IronJS.Runtime.Binders;
using IronJS.Runtime.Builtins;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Runtime
{
    public class Context
    {
        static readonly public MethodInfo MiCreateObject = typeof(Context).GetMethod("CreateObject", Type.EmptyTypes);
        static readonly public MethodInfo MiCreateString = typeof(Context).GetMethod("CreateString", new[] { typeof(object) });
        static readonly public MethodInfo MiCreateNumber = typeof(Context).GetMethod("CreateNumber", new[] { typeof(object) });
        static readonly public MethodInfo MiCreateBoolean = typeof(Context).GetMethod("CreateBoolean", new[] { typeof(object) });
        static readonly public MethodInfo MiCreateFunction = typeof(Context).GetMethod("CreateFunction", new[] { typeof(Scope), typeof(Lambda) });
        static readonly public MethodInfo MiCreateRegExp = typeof(Context).GetMethod("CreateRegExp", new[] { typeof(object), typeof(object) });

        public Object_ctor ObjectConstructor { get; protected set; }
        public Function_ctor FunctionConstructor { get; protected set; }
        public Array_ctor ArrayConstructor { get; protected set; }
        public String_ctor StringConstructor { get; protected set; }
        public Boolean_ctor BooleanConstructor { get; protected set; }
        public Number_ctor NumberConstructor { get; protected set; }
        public RegExp_ctor RegExpContructor { get; protected set; }
        public Math_obj MathObject { get; protected set; }

        protected Context()
        {
            FunctionConstructor = Function_ctor.Create(this);
            ObjectConstructor = Object_ctor.Create(this);
            ArrayConstructor = Array_ctor.Create(this);
            StringConstructor = String_ctor.Create(this);
            BooleanConstructor = Boolean_ctor.Create(this);
            NumberConstructor = Number_ctor.Create(this);
            RegExpContructor = RegExp_ctor.Create(this);
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
            globals.Global("eval", new Global_obj_eval(this));
            globals.Global("parseInt", new Global_obj_parseInt(this));
            globals.Global("parseFloat", new Global_obj_parseFloat(this));
            globals.Global("isNaN", new Global_obj_isNaN(this));
            globals.Global("isFinite", new Global_obj_isFinite(this));
            globals.Global("encodeURI", new Global_obj_encodeURI(this));
            globals.Global("decodeURI", new Global_obj_decodeURI(this));
            globals.Global("encodeURIComponent", new Global_obj_encodeURIComponent(this));
            globals.Global("decodeURIComponent", new Global_obj_decodeURIComponent(this));
        }

        #region Object creators

        public IObj CreateRegExp(object pattern, object flags)
        {
            return RegExpContructor.Construct(pattern, flags);
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

        public Obj CreateObject()
        {
            var obj = new Obj();

            obj.Context = this;
            obj.Class = ObjClass.Object;

            return obj;
        }

        public UserFunction CreateFunction(Scope scope, Lambda lambda)
        {
            var obj = new UserFunction(scope, lambda);

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

        static public Context Create()
        {
            return new Context();
        }

        static internal Et EtCreateFunction(Context context, Et scope, Et lambda)
        {
            return Et.Call(
                Et.Constant(context),
                MiCreateFunction,
                scope,
                lambda
            );
        }

        #endregion
    }
}

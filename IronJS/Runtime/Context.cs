using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Js;
using IronJS.Runtime.Binders;
using System.Linq.Expressions;
using System.Dynamic;
using System.Reflection;
using System.Globalization;
//using IronJS.Runtime.Builtins;

namespace IronJS.Runtime
{
    using Et = System.Linq.Expressions.Expression;
    using EtParam = System.Linq.Expressions.ParameterExpression;
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using IronJS.Runtime.Builtins;

    public class Context
    {
        internal Scope BuiltinsGlobals { get; private set; }

        public ObjectCtor ObjectConstructor { get; protected set; }

        public IFunction FunctionPrototype { get; protected set; }
        public IFunction FunctionConstructor { get; protected set; }

        public IObj BooleanPrototype { get; protected set; }
        public IFunction BooleanConstructor { get; protected set; }

        public IObj NumberPrototype { get; set; }
        public IFunction NumberConstructor { get; protected set; }

        public IObj StringPrototype { get; set; }
        public IFunction StringConstructor { get; protected set; }

        public IObj ArrayPrototype { get; protected set; }
        public IFunction ArrayConstructor { get; protected set; }

        public IObj Math { get; protected set; }

        protected Context()
        {
            BuiltinsGlobals = Scope.CreateGlobal(this);
            ObjectConstructor = ObjectCtor.Create(this);

            /*
            // Function.prototype and Function
            FunctionPrototype = FunctionObject.CreatePrototype(this);
            FunctionConstructor = FunctionObject.CreateConstructor(this);

            // Array.prototype and Array
            ArrayPrototype = ArrayObject.CreatePrototype(this);
            ArrayConstructor = ArrayObject.CreateConstructor(this);

            // Boolean.prototype
            BooleanPrototype = BooleanObject.CreatePrototype(this);

            // Number.prototype
            NumberPrototype = NumberObject.CreatePrototype(this);
            */
            // Math
            // Math = MathObject.Create(this);
        }

        public void Setup(Scope globals)
        {
            globals.Global("Object", ObjectConstructor);
            globals.Global("undefined", Js.Undefined.Instance);
            globals.Global("Infinity", double.PositiveInfinity);
            globals.Global("NaN", double.NaN);

            /*
            globals.Put("Function", FunctionConstructor);
            globals.Put("Array", ArrayConstructor);
            globals.Put("Number", NumberConstructor);
            globals.Put("Boolean", BooleanConstructor);
            globals.Put("undefined", Js.Undefined.Instance);
            globals.Put("Infinity", double.PositiveInfinity);
            globals.Put("NaN", double.NaN);
            globals.Put("Math", Math);
            globals.Put("globals", globals);

            setup(globals);
            target(globals);

            return globals;
            */
        }

        #region Object creators

        public Obj CreateObject()
        {
            var obj = new Obj();

            obj.Context = this;
            obj.Class = ObjClass.Object;

            return obj;
        }

        public IObj CreateArray()
        {
            var obj = new ArrayObj();

            obj.Context = this;
            obj.Class = ObjClass.Array;
            obj.Prototype = ArrayPrototype;

            return obj;
        }

        public IFunction CreateFunction(Scope scope, Lambda lambda)
        {
            var obj = new Function(scope, lambda);

            var protoObj = CreateObject();
            protoObj.SetOwnProperty("constructor", obj);

            obj.Context = this;
            obj.Class = ObjClass.Function;
            obj.Prototype = FunctionPrototype;
            obj.SetOwnProperty("prototype", protoObj);

            return obj;
        }

        public IValueObj CreateString(string value)
        {
            var obj = new ValueObj(value);

            obj.Context = this;
            obj.Class = ObjClass.String;
            obj.Prototype = null; //TODO: String.prototype

            return obj;
        }

        public IValueObj CreateNumber(double value)
        {
            var obj = new ValueObj(value);

            obj.Context = this;
            obj.Class = ObjClass.Number;
            obj.Prototype = NumberPrototype;

            return obj;
        }

        public IValueObj CreateBoolean(bool value)
        {
            var obj = new ValueObj(value);

            obj.Context = this;
            obj.Class = ObjClass.Boolean;
            obj.Prototype = BooleanPrototype;

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
            static public MethodInfo CreateString = typeof(Context).GetMethod("CreateString", new[] { typeof(string) });
            static public MethodInfo CreateNumber = typeof(Context).GetMethod("CreateNumber", new[] { typeof(double) });
            static public MethodInfo CreateBoolean = typeof(Context).GetMethod("CreateBoolean", new[] { typeof(bool) });
        }

        #endregion

    }
}

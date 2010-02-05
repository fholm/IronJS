using System;
using System.Dynamic;

using System.Reflection;
using IronJS.Runtime.Binders;
using IronJS.Runtime.Builtins;
using IronJS.Runtime.Js;

using System.Collections.Generic;
using IronJS.Runtime.Js.Descriptors;
using Microsoft.Scripting.Ast;

namespace IronJS.Runtime
{
    public class Context
    {
        static readonly public MethodInfo MiCreateObject = typeof(Context).GetMethod("CreateObject");
        static readonly public MethodInfo MiCreateArray = typeof(Context).GetMethod("CreateArray", new[] { typeof(object[]) });
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

        public Context()
        {
            /*
             * Because of all the circular dependencies
             * between the native objects we have to
             * first create the Function and Function.prototype object
             * then create the Object and Object.prototype
             * */
            FunctionConstructor = new Function_ctor(this);
            ObjectConstructor = new Object_ctor(this);

            /*
             * Then connect the dependencies between 
             * Object/Function manually
             * */
            ObjectConstructor.Prototype = FunctionConstructor.Function_prototype;
            FunctionConstructor.Prototype = FunctionConstructor.Function_prototype;
            FunctionConstructor.Function_prototype.Prototype = ObjectConstructor.Object_prototype;

            /*
             * And after that, we can create the rest
             * of the native objects
             * */
            ArrayConstructor = new Array_ctor(this);
            StringConstructor = new String_ctor(this);
            BooleanConstructor = new Boolean_ctor(this);
            NumberConstructor = new Number_ctor(this);
            RegExpContructor = new RegExp_ctor(this);
            MathObject = new Math_obj(this);
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

        public IObj CreateArray(object[] args)
        {
            return ArrayConstructor.Construct(args);
        }

        public IObj CreateObject()
        {
            return ObjectConstructor.Construct();
        }

        public UserFunction CreateFunction(Scope scope, Lambda lambda)
        {
            var obj = new UserFunction(scope, lambda);

            var protoObj = ObjectConstructor.Construct();
            protoObj.Set("constructor", 
                new NativeProperty(protoObj, obj, false, true, false)
            );

            obj.Context = this;
            obj.Class = ObjClass.Function;
            obj.Prototype = FunctionConstructor.Function_prototype;
            obj.Set("prototype", protoObj);

            return obj;
        }

        #endregion

        #region Binders

        Dictionary<ExpressionType, JsBinaryOpBinder> _binaryOpBinders 
            = new Dictionary<ExpressionType, JsBinaryOpBinder>();
        internal JsBinaryOpBinder CreateBinaryOpBinder(ExpressionType op)
        {
            if (_binaryOpBinders.ContainsKey(op))
                return _binaryOpBinders[op];

            return _binaryOpBinders[op] = new JsBinaryOpBinder(op, this);
        }

        Dictionary<ExpressionType, JsUnaryOpBinder> _unaryOpBinders
            = new Dictionary<ExpressionType, JsUnaryOpBinder>();
        internal JsUnaryOpBinder CreateUnaryOpBinder(ExpressionType op)
        {
            if (_unaryOpBinders.ContainsKey(op))
                return _unaryOpBinders[op];

            return _unaryOpBinders[op] = new JsUnaryOpBinder(op, this);
        }

        Dictionary<Type, JsConvertBinder> _convertBinders
            = new Dictionary<Type, JsConvertBinder>();
        internal JsConvertBinder CreateConvertBinder(Type type)
        {
            if (_convertBinders.ContainsKey(type))
                return _convertBinders[type];

            return _convertBinders[type] = new JsConvertBinder(type, this);
        }

        Dictionary<CallInfo, JsGetIndexBinder> _getIndexBinders
            = new Dictionary<CallInfo, JsGetIndexBinder>();
        internal JsGetIndexBinder CreateGetIndexBinder(CallInfo callInfo)
        {
            if (_getIndexBinders.ContainsKey(callInfo))
                return _getIndexBinders[callInfo];

            return _getIndexBinders[callInfo] = new JsGetIndexBinder(callInfo, this);
        }

        Dictionary<CallInfo, JsSetIndexBinder> _setIndexBinders
            = new Dictionary<CallInfo, JsSetIndexBinder>();
        internal JsSetIndexBinder CreateSetIndexBinder(CallInfo callInfo)
        {
            if (_setIndexBinders.ContainsKey(callInfo))
                return _setIndexBinders[callInfo];

            return _setIndexBinders[callInfo] = new JsSetIndexBinder(callInfo, this);
        }

        Dictionary<CallInfo, JsDeleteIndexBinder> _deleteIndexBinders
            = new Dictionary<CallInfo, JsDeleteIndexBinder>();
        internal JsDeleteIndexBinder CreateDeleteIndexBinder(CallInfo callInfo)
        {
            if (_deleteIndexBinders.ContainsKey(callInfo))
                return _deleteIndexBinders[callInfo];

            return _deleteIndexBinders[callInfo] = new JsDeleteIndexBinder(callInfo, this);
        }

        Dictionary<object, JsGetMemberBinder> _getMemberBinders
            = new Dictionary<object, JsGetMemberBinder>();
        internal JsGetMemberBinder CreateGetMemberBinder(object name)
        {
            if (_getMemberBinders.ContainsKey(name))
                return _getMemberBinders[name];

            return _getMemberBinders[name] = new JsGetMemberBinder(name, this);
        }

        Dictionary<object, JsSetMemberBinder> _setMemberBinders
            = new Dictionary<object, JsSetMemberBinder>();
        internal JsSetMemberBinder CreateSetMemberBinder(object name)
        {
            if (_setMemberBinders.ContainsKey(name))
                return _setMemberBinders[name];

            return _setMemberBinders[name] = new JsSetMemberBinder(name, this);
        }

        Dictionary<object, JsDeleteMemberBinder> _deleteMemberBinders
            = new Dictionary<object, JsDeleteMemberBinder>();
        internal JsDeleteMemberBinder CreateDeleteMemberBinder(object name)
        {
            if (_deleteMemberBinders.ContainsKey(name))
                return _deleteMemberBinders[name];

            return _deleteMemberBinders[name] = new JsDeleteMemberBinder(name, this);
        }

        Dictionary<CallInfo, JsInvokeBinder> _invokeBinders
            = new Dictionary<CallInfo, JsInvokeBinder>();
        internal JsInvokeBinder CreateInvokeBinder(CallInfo callInfo)
        {
            if (_invokeBinders.ContainsKey(callInfo))
                return _invokeBinders[callInfo];

            return _invokeBinders[callInfo] = new JsInvokeBinder(callInfo, this);
        }

        Dictionary<CallInfo, JsCreateInstanceBinder> _createInstanceBinders
            = new Dictionary<CallInfo, JsCreateInstanceBinder>();
        internal JsCreateInstanceBinder CreateInstanceBinder(CallInfo callInfo)
        {
            if (_createInstanceBinders.ContainsKey(callInfo))
                return _createInstanceBinders[callInfo];

            return _createInstanceBinders[callInfo] = new JsCreateInstanceBinder(callInfo, this);
        }

        Dictionary<int, JsInvokeMemberBinder> _invokeMemberBinder
            = new Dictionary<int, JsInvokeMemberBinder>();
        internal JsInvokeMemberBinder CreateInvokeMemberBinder(object name, CallInfo callInfo)
        {
            // Stolen from SymPL who stole it from the DLR
            var key = 0x28000000 ^ name.GetHashCode() ^ callInfo.GetHashCode();

            if (_invokeMemberBinder.ContainsKey(key))
                return _invokeMemberBinder[key];

            return _invokeMemberBinder[key] = new JsInvokeMemberBinder(name, callInfo, this);
        }

        #endregion
    }
}

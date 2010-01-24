using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Builtins
{
    public class Object_ctor : Obj, IFunction
    {
        public IObj Object_prototype { get; private set; }

        protected Object_ctor(Context context)
        {
            Context = context;
            Object_prototype = CreatePrototype();
            Put("prototype", Object_prototype);
        }

        public IObj Construct()
        {
            return Construct(null);
        }

        protected IObj CreatePrototype()
        {
            var obj = new Obj();

            obj.Class = ObjClass.Object;
            obj.Prototype = null;
            obj.Context = Context;
            obj.SetOwnProperty("constructor", this);
            obj.SetOwnProperty("toString", new Object_prototype_toString(Context));
            obj.SetOwnProperty("valueOf", new Object_prototype_valueOf(Context));
            obj.SetOwnProperty("hasOwnProperty", new Object_prototype_hasOwnProperty(Context));
            obj.SetOwnProperty("isPrototypeOf", new Object_prototype_isPrototypeOf(Context));
            obj.SetOwnProperty("propertyIsEnumerable", new Object_prototype_propertyIsEnumerable(Context));
            obj.SetOwnProperty("toLocaleString", new Object_prototype_toLocaleString(Context));

            return obj;
        }

        #region IFunction Members

        // 15.2.1.1
        public object Call(IObj that, object[] args)
        {
            // step 1
            if (args.Length == 0 
                || args[0] == null 
                || args[0] is Undefined)
                return Construct(args);

            // step 2
            return JsTypeConverter.ToObject(args[0], Context);
        }

        public IObj Construct(object[] args)
        {
            // step 8 (verification)
            if (args != null
                && args.Length > 0 
                && args[0] != null 
                && !(args[0] is Undefined))
            {
                var value = args[0];

                // step 3
                if (value is IObj)
                    return (IObj)value;

                // step 5, 6 and 7
                if (value is double || value is string || value is bool)
                    return JsTypeConverter.ToObject(value, Context);

                // step 4
                throw new NotImplementedException("Can't convert value of type '" + args[0].GetType() + "' to IObj");
            }

            // step 8
            var obj = Context.CreateObject();
            obj.Prototype = Object_prototype;
            return obj;
        }

        public bool HasInstance(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDynamicMetaObjectProvider Members

        public Meta GetMetaObject(Et parameter)
        {
            return new IFunctionMeta(parameter, this);
        }

        #endregion

        #region Static

        static public Object_ctor Create(Context context)
        {
            return new Object_ctor(context);
        }

        #endregion 
    }
}

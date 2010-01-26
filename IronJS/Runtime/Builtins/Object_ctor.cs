using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Builtins
{
    public class Object_ctor : NativeConstructor
    {
        public IObj Object_prototype { get; private set; }

        public Object_ctor(Context context)
            : base(context)
        {
            Object_prototype = new Object_prototype(context);
            Object_prototype.Set("constructor", this);

            this.Set("prototype", Object_prototype);
        }

        public IObj Construct()
        {
            return Construct(null);
        }

        // 15.2.1.1
        override public object Call(IObj that, object[] args)
        {
            // step 1
            if (args.Length == 0 
                || args[0] == null 
                || args[0] is Undefined)
                return Construct(args);

            // step 2
            return JsTypeConverter.ToObject(args[0], Context);
        }

        override public IObj Construct(object[] args)
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
            var obj = new Obj();

            obj.Class = ObjClass.Object;
            obj.Context = Context;
            obj.Prototype = Object_prototype;

            return obj;
        }
    }
}

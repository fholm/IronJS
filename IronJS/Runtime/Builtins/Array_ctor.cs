using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Builtins
{
    public class Array_ctor : NativeConstructor
    {
        public IObj Array_prototype { get; private set; }

        public Array_ctor(Context context)
            : base(context)
        {
            // 15.4.3
            Prototype = context.FunctionConstructor.Function_prototype;

            Array_prototype = new Array_prototype(context);
            Array_prototype.SetOwn("constructor", this);

            // 15.4.3
            SetOwn("length", 1.0D);

            // 15.4.3.1
            SetOwn("prototype", Array_prototype);
        }

        public IObj Construct()
        {
            return Construct(null);
        }

        #region IFunction Members

        override public object Call(IObj that, object[] args)
        {
            // 15.4.1.1
            return Construct(args);
        }

        override public IObj Construct(object[] args)
        {
            var arrayObj = new ArrayObj();

            arrayObj.Class = ObjClass.Array;
            arrayObj.Prototype = Array_prototype;
            arrayObj.Context = Context;

            if (args != null && args.Length != 1)
            {
                // 15.4.2.1
                for (int i = 0; i < args.Length; ++i)
                    arrayObj.SetOwn((double)i, args[i]);
            }
            else if(args != null)
            {
                // 15.4.2.2
                var len = JsTypeConverter.ToNumber(args[0]);

                if ((double)JsTypeConverter.ToInt32(len) == len)
                {
                    arrayObj.SetOwn("length", len);
                }
                else
                {
                    throw new ShouldThrowTypeError();
                }
            }

            return arrayObj;
        }

        #endregion
    }
}

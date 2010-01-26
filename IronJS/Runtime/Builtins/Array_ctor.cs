using IronJS.Runtime.Js;
using IronJS.Runtime.Js.Descriptors;
using IronJS.Runtime.Utils;

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
            Array_prototype.Set("constructor", this);

            // 15.4.3
            this.Set("length", 1);

            // 15.4.3.1
            this.Set("prototype", 
                new NativeProperty(
                    this, Array_prototype, 
                    isReadOnly: true, 
                    isDeletable: false, 
                    isEnumerable: false
                )
            );
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
            var arrayObj = new JsArray();

            arrayObj.Class = ObjClass.Array;
            arrayObj.Prototype = Array_prototype;
            arrayObj.Context = Context;

            if (args != null && args.Length != 1)
            {
                // 15.4.2.1
                for (int i = 0; i < args.Length; ++i)
                    arrayObj.Set(i, args[i]);
            }
            else if(args != null)
            {
                // 15.4.2.2
                var len = JsTypeConverter.ToNumber(args[0]);

                if ((double)JsTypeConverter.ToInt32(len) == len)
                {
                    arrayObj.Set("length", len);
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

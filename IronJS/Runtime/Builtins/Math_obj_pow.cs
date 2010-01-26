using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using IronJS.Runtime.Js.Descriptors;

namespace IronJS.Runtime.Builtins
{
    class Math_obj_pow : NativeFunction
    {
        public Math_obj_pow(Context context)
            : base(context)
        {
            Set("length", new UserProperty(this, 2.0D));
        }

        public override object Call(IObj that, object[] args)
        {
            if (!HasArgs(args, 2))
                throw new ArgumentException();

            return Math.Pow(
                JsTypeConverter.ToNumber(args[0]),
                JsTypeConverter.ToNumber(args[1])
            );
        }
    }
}
using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class Math_obj_atan2 : NativeFunction
    {
        public Math_obj_atan2(Context context)
            : base(context)
        {
            SetOwnProperty("length", 2.0D);
        }

        public override object Call(IObj that, object[] args)
        {
            if (!HasArgs(args, 2))
                throw new ArgumentException();

            return Math.Atan2(
                JsTypeConverter.ToNumber(args[0]),
                JsTypeConverter.ToNumber(args[1])
            );
        }
    }
}
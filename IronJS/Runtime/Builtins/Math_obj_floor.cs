using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class Math_obj_floor : NativeFunction
    {
        public Math_obj_floor(Context context)
            : base(context)
        {
            SetOwnProperty("length", 1.0D);
        }

        public override object Call(IObj that, object[] args)
        {
            if (!HasArgs(args))
                throw new ArgumentException();

            return Math.Floor(JsTypeConverter.ToNumber(args[0]));
        }
    }
}
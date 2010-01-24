using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class Math_obj_acos : NativeFunction
    {
        public Math_obj_acos(Context context)
            : base(context)
        {
            SetOwnProperty("length", 1.0D);
        }

        public override object Call(IObj that, object[] args)
        {
            if (!HasArgs(args))
                throw new ArgumentException();

            return Math.Acos(JsTypeConverter.ToNumber(args[0]));
        }
    }
}
using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class Math_obj_min : NativeFunction
    {
        public Math_obj_min(Context context)
            : base(context)
        {
            SetOwnProperty("length", 1.0D);
        }

        public override object Call(IObj that, object[] args)
        {
            var max = double.PositiveInfinity;

            foreach (var arg in args)
                max = Math.Min(max, JsTypeConverter.ToNumber(args));

            return max;
        }
    }
}
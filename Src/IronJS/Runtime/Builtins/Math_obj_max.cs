using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using IronJS.Runtime.Js.Descriptors;

namespace IronJS.Runtime.Builtins
{
    class Math_obj_max : NativeFunction
    {
        public Math_obj_max(Context context)
            : base(context)
        {
            Set("length", new UserProperty(this, 1.0D));
        }

        public override object Call(IObj that, object[] args)
        {
            var max = double.NegativeInfinity;

            foreach(var arg in args)
                max = Math.Max(max, JsTypeConverter.ToNumber(args));

            return max;
        }
    }
}
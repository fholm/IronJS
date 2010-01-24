using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class Global_obj_isFinite : NativeFunction
    {
        public Global_obj_isFinite(Context context)
            : base(context)
        {
            SetOwnProperty("length", 1.0D);
        }

        public override object Call(IObj that, object[] args)
        {
            if (!HasArgs(args))
                throw new ArgumentException();

            var num = JsTypeConverter.ToNumber(args[0]);
            return !double.IsInfinity(num) && !double.IsNaN(num);
        }
    }
}
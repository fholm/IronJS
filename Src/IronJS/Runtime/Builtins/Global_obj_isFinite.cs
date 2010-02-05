using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using IronJS.Runtime.Js.Descriptors;

namespace IronJS.Runtime.Builtins
{
    class Global_obj_isFinite : NativeFunction
    {
        public Global_obj_isFinite(Context context)
            : base(context)
        {
            Set("length", new UserProperty(this, 1.0D));
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
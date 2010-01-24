using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class Global_obj_parseInt : NativeFunction
    {
        public Global_obj_parseInt(Context context)
            : base(context)
        {
            SetOwnProperty("length", 1.0D);
        }

        public override object Call(IObj that, object[] args)
        {
            //TODO: This is just a quick version that handles most cases, not 100% yet

            if (!HasArgs(args) || args[0] == null || args[0] is Undefined)
                return Undefined.Instance;

            var strNumber = JsTypeConverter.ToString(args[0]).Trim();
            int sign = 1;
            int radix = 10;

            if(HasArgs(args, 2) && (args[1] != null) && !(args[1] is Undefined))
                radix = JsTypeConverter.ToInt32(args[1]);

            if(strNumber.Length == 0 || radix < 2 || radix > 36)
                return double.NaN;

            if(strNumber[0] == '-')
            {
                strNumber = strNumber.Substring(1);
                sign = -1;
            }

            try
            {
                return (double)(sign * Convert.ToInt32(strNumber, radix));
            }
            catch(FormatException)
            {
                return double.NaN;
            }
        }
    }
}
using System;
using IronJS.Extensions;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class Global_obj_parseFloat : NativeFunction
    {
        public Global_obj_parseFloat(Context context)
            : base(context)
        {
            SetOwnProperty("length", 1.0D);
        }

        public override object Call(IObj that, object[] args)
        {
            if (!HasArgs(args))
                throw new ArgumentException();

            double result;

            if (DoubleExt.TryParseInvariant(JsTypeConverter.ToString(args[0]), out result))
                return result;

            return double.NaN;
        }
    }
}
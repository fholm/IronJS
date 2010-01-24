using System;
using System.Globalization;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class Number_prototype_toExponential : NativeFunction
    {
        public Number_prototype_toExponential(Context context)
            : base(context)
        {
            SetOwnProperty("length", 1.0D);
        }

        public override object Call(IObj that, object[] args)
        {
            if (that.Class != ObjClass.Number || !that.HasValue())
                throw new ShouldThrowTypeError();

            var fractions = HasArgs(args) ? JsTypeConverter.ToInt32(args[0]) : 0;
            var dbl = JsTypeConverter.ToNumber((that as ValueObj).Value);
            string format = "#." + new String('0', fractions) + "e+0";
            return dbl.ToString(format, CultureInfo.InvariantCulture);
        }

    }
}

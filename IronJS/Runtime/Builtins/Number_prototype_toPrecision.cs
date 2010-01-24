using System.Globalization;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class Number_prototype_toPrecision : NativeFunction
    {
        public Number_prototype_toPrecision(Context context)
            : base(context)
        {
            SetOwnProperty("length", 1.0D);
        }

        public override object Call(IObj that, object[] args)
        {
            if (that.Class != ObjClass.Number || !that.HasValue())
                throw new ShouldThrowTypeError();

            var dbl = JsTypeConverter.ToNumber((that as ValueObj).Value);

            if (double.IsNaN(dbl) || double.IsInfinity(dbl) || !HasArgs(args) || args[0] is Undefined)
                return JsTypeConverter.ToString(dbl);

            var precision = HasArgs(args) ? JsTypeConverter.ToInt32(args[0]) : 1;
            var asString = dbl.ToString("e23", CultureInfo.InvariantCulture);
            var numDecimals = asString.IndexOfAny(new char[] { '.', 'e' });

            precision -= (numDecimals == -1 ? asString.Length : numDecimals);

            return dbl.ToString("f" + (precision < 1 ? 1 : precision), CultureInfo.InvariantCulture);
        }

    }
}

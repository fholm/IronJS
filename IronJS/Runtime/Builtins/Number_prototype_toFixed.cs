using System.Globalization;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using IronJS.Runtime.Js.Descriptors;

namespace IronJS.Runtime.Builtins
{
	class Number_prototype_toFixed : NativeFunction
    {
        public Number_prototype_toFixed(Context context)
            : base(context)
        {
            Set("length", new UserProperty(this, 1.0D));
        }

        public override object Call(IObj that, object[] args)
        {
            if (that.Class != ObjClass.Number || !that.HasValue())
                throw new ShouldThrowTypeError();

            var fractions = HasArgs(args) ? JsTypeConverter.ToInt32(args[0]) : 0;
            var dbl = JsTypeConverter.ToNumber((that as ValueObj).Value);
            return dbl.ToString("f" + fractions, CultureInfo.InvariantCulture);
        }

    }
}

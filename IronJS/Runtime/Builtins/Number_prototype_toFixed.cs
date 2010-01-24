using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using System.Globalization;

namespace IronJS.Runtime.Builtins
{
	class Number_prototype_toFixed : NativeFunction
    {
        public Number_prototype_toFixed(Context context)
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
            return dbl.ToString("f" + fractions, CultureInfo.InvariantCulture);
        }

    }
}

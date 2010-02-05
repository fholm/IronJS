using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class String_prototype_charCodeAt : NativeFunction
    {
        public String_prototype_charCodeAt(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            if (!HasArgs(args))
                throw new ArgumentException();

            var str = JsTypeConverter.ToString(that);
            var pos = JsTypeConverter.ToInt32(args[0]);

            if (pos < str.Length)
                return (double)(int)str[pos];

            return double.NaN;
        }
    }
}

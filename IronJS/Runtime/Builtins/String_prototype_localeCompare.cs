using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class String_prototype_localeCompare : NativeFunction
    {
        public String_prototype_localeCompare(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            if (!HasArgs(args))
                throw new ArgumentException();

            var target = JsTypeConverter.ToString(args[0]);
            var str = JsTypeConverter.ToString(that);

            return str.CompareTo(target);
        }

    }
}

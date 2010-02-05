using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class String_prototype_indexOf : NativeFunction
    {
        public String_prototype_indexOf(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            if (!HasArgs(args))
                throw new ArgumentException();

            var target = JsTypeConverter.ToString(that);
            var search = JsTypeConverter.ToString(args[0]);
            var index = JsTypeConverter.ToInt32(args.Length > 1 ? args[1] : 0);

            return (double) target.IndexOf(search, index);
        }

    }
}

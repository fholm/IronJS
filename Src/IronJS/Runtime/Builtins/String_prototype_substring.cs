using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class String_prototype_substring : NativeFunction
    {
        public String_prototype_substring(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            if (!HasArgs(args))
                throw new ArgumentException();

            var str = JsTypeConverter.ToString(that);
            var len = str.Length;
            var start = JsTypeConverter.ToInt32(args[0]);
            var end = args.Length > 1 ? JsTypeConverter.ToInt32(args[1]) : len;

            var step5 = Math.Min(Math.Max(start, 0), len);
            var step6 = Math.Min(Math.Max(end, 0), len);
            var step7 = Math.Min(step5, step6);
            var step8 = Math.Max(step5, step6);

            return str.Substring(step7, step8 - step7);
        }
    }
}

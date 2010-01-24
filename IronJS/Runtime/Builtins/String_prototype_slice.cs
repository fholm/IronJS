using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class String_prototype_slice : NativeFunction
    {
        public String_prototype_slice(Context context)
            : base(context)
        {
            SetOwnProperty("length", 0.0D);
        }

        public override object Call(IObj that, object[] args)
        {
            if (!HasArgs(args))
                throw new ArgumentException();

            var str = JsTypeConverter.ToString(that);
            var len = str.Length;
            var start = JsTypeConverter.ToInt32(args[0]);
            var end = args.Length > 1 ? JsTypeConverter.ToInt32(args[1]) : args.Length;

            start = start < 0 ? Math.Max(len + start, 0) : Math.Min(start, len);
            end = end < 0 ? Math.Max(len + end, 0) : Math.Min(end, len);

            var count = Math.Max(end - start, 0);

            return str.Substring(start, count);
        }
    }
}

using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class Array_prototype_slice : NativeFunction
    {
        public Array_prototype_slice(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            if(!HasArgs(args, 1))
                throw new NotImplementedException();

            var newArray = Context.ArrayConstructor.Construct();
            var len = JsTypeConverter.ToNumber(that.Get("length"));
            var start = JsTypeConverter.ToNumber(args[0]);

            var ks = (start < 0.0)
                    ? Math.Max(len + start, 0.0D)
                    : Math.Min(start, len);

            var end = args.Length > 1 ? JsTypeConverter.ToNumber(args[1]) : len;

            var ke = (end < 0.0)
                     ? Math.Max(len + end, 0.0D)
                     : Math.Min(end, len);

            var n = 0.0D;
            object val;

            while(ks < ke)
            {
                if (that.TryGet(ks, out val))
                    newArray.SetOwnProperty(n, val);

                ++ks;
                ++n;
            }

            return newArray;
        }
    }
}

using System.Collections.Generic;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class Array_prototype_sort : NativeFunction
    {
        public Array_prototype_sort(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            var len = JsTypeConverter.ToNumber(that.Get("length"));
            var vals = new List<object>();
            object val;

            for (var d = 0.0D; d < len; ++d)
            {
                if (that.TryGet(d, out val))
                    vals.Add(val);
            }
            if (!HasArgs(args))
            {
                vals.Sort((a, b) => {
                    var an = JsTypeConverter.ToNumber(JsTypeConverter.ToPrimitive(a));
                    var bn = JsTypeConverter.ToNumber(JsTypeConverter.ToPrimitive(b));
                    return (int)an - (int)bn;
                });
            }
            else
            {
                var func = args[0] as IFunction;
                vals.Sort((a, b) => (int)(double)func.Call(that, new[] { a, b }) );
            }

            for (var d = 0.0D; d < len; ++d)
            {
                if ((int)d < vals.Count)
                {
                    that.SetOwnProperty(d, vals[(int)d]);
                }
                else
                {
                    that.Delete(d);
                }
            }

            return that;
        }
    }
}

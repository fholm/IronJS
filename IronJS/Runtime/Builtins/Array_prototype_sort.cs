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
            IDescriptor<IObj> descriptor;

            for (var d = 0; d < len; ++d)
            {
                if (that.Get(d, out descriptor))
                    vals.Add(descriptor.Get());
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

            for (var d = 0; d < len; ++d)
            {
                if ((int)d < vals.Count)
                {
                    that.Set(d, vals[(int)d]);
                }
                else
                {
                    that.TryDelete(d);
                }
            }

            return that;
        }
    }
}

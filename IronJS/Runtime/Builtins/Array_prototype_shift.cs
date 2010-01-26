using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class Array_prototype_shift : NativeFunction
    {
        public Array_prototype_shift(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            var len = JsTypeConverter.ToNumber(that.Get("length"));

            if (len == 0.0D)
            {
                that.Set("length", 0.0D);
                return Undefined.Instance;
            }

            object shifted = that.Get(0.0D);
            var d = 1.0D;

            for (; d < len; ++d)
            {
                IDescriptor<IObj> descriptor;

                if (that.Get(d, out descriptor))
                {
                    that.Set(d - 1.0D, descriptor.Get());
                    that.TryDelete(d);
                }
                else
                {
                    that.TryDelete(d - 1.0D);
                }
            }

            that.Set("length", d - 1.0D);
            return shifted;
        }
    }
}

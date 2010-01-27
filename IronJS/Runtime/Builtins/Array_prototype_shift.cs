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
            var len = JsTypeConverter.ToInt32(that.Get("length"));

            if (len == 0)
            {
                that.Set("length", 0);
                return Undefined.Instance;
            }

            object shifted = that.Get(0);
            var n = 1;

            for (; n < len; ++n)
            {
                IDescriptor<IObj> descriptor;

                if (that.Get(n, out descriptor))
                {
                    that.Set(n - 1, descriptor.Get());
                    that.TryDelete(n);
                }
                else
                {
                    that.TryDelete(n - 1);
                }
            }

            that.Set("length", n - 1);
            return shifted;
        }
    }
}

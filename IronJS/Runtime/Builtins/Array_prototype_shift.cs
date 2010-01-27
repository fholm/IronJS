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
            var d = 0;

            for (; d < len; ++d)
            {
                IDescriptor<IObj> descriptor;

                if (that.Get(d, out descriptor))
                {
                    that.Set(d - 1, descriptor.Get());
                    that.TryDelete(d);
                }
                else
                {
                    that.TryDelete(d - 1);
                }
            }

            that.Set("length", d - 1);
            return shifted;
        }
    }
}

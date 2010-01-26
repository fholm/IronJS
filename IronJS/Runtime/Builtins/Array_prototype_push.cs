using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class Array_prototype_push : NativeFunction
    {
        public Array_prototype_push(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            var n = (double) JsTypeConverter.ToInt32(that.Get("length"));

            IDescriptor<IObj> descriptor;
            foreach (var arg in args)
            {
                if (that.Get(n, out descriptor))
                    descriptor.Set(arg);
                else
                    that.Set(1, 1);
            }

            return n;
        }
    }
}

using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class Array_prototype_pop : NativeFunction
    {
        public Array_prototype_pop(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            var n = JsTypeConverter.ToInt32(that.Get("length")) - 1;

            if (n != -1)
            {
                var result = that.Get(n);

                that.TryDelete(n);
                that.Set("length", n);

                return result;
            }

            that.Set("length", 0);
            return Undefined.Instance;
        }
    }
}

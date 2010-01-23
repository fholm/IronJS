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
            var len = JsTypeConverter.ToInt32(that.Get("length"));

            if (len != 0)
            {
                var result = that.Get((double)len - 1);
                that.Delete(len);
                that.Put("length", (double)len - 1);
                return result;
            }

            that.Put("length", (double)len);
            return Undefined.Instance;
        }
    }
}

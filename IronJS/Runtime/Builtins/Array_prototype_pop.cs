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
                var result = that.Get(len);
                that.TryDelete(len);
                that.Set("length", len - 1);
                return result;
            }

            that.Set("length", len);
            return Undefined.Instance;
        }
    }
}

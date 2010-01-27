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
            var n = JsTypeConverter.ToInt32(that.Get("length"));

            foreach (var arg in args)
                that.Set(n++, arg);

            return n;
        }
    }
}

using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class String_prototype_toUpperCase : NativeFunction
    {
        public String_prototype_toUpperCase(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            return JsTypeConverter.ToString(that).ToUpperInvariant();
        }
    }
}

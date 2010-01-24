using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class String_prototype_toLocaleLowerCase : NativeFunction
    {
        public String_prototype_toLocaleLowerCase(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            return JsTypeConverter.ToString(that).ToLower();
        }
    }
}

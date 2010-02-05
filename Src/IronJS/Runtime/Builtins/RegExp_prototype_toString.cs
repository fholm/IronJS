using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class RegExp_prototype_toString : NativeFunction
    {
        public RegExp_prototype_toString(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            if (!(that is RegExpObj))
                throw new ShouldThrowTypeError();

            var regExpObj = that as RegExpObj;
            var multiline = JsTypeConverter.ToBoolean(regExpObj.Get("multiline"));
            var global = JsTypeConverter.ToBoolean(regExpObj.Get("global"));
            var ignoreCase = JsTypeConverter.ToBoolean(regExpObj.Get("ignoreCase"));

            return "/" + JsTypeConverter.ToString(that.Get("source")) + "/" +
                    (multiline ? "m" : "") +
                    (global ? "g" : "") +
                    (ignoreCase ? "i" : "");
        }
    }
}

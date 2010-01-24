using System;
using System.Web;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class Global_obj_encodeURIComponent : NativeFunction
    {
        public Global_obj_encodeURIComponent(Context context)
            : base(context)
        {
            SetOwnProperty("length", 1.0D);
        }

        public override object Call(IObj that, object[] args)
        {
            if (!HasArgs(args) || args[0] == null || args[0] is Undefined)
                throw new ArgumentException();

            var encoded = HttpUtility.UrlEncode(JsTypeConverter.ToString(args[0]));

            //TODO: unroll loop
            foreach (char c in Global_obj_encodeURI.RESERVED_COMPONENT)
            {
                var str = c.ToString();
                encoded = encoded.Replace(HttpUtility.UrlEncode(str), str);
            }

            return encoded;
        }
    }
}
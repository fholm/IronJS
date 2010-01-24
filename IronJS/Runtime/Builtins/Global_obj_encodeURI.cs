using System;
using System.Web;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class Global_obj_encodeURI : NativeFunction
    {
        internal static char[] RESERVED = new char[] { '#', ';', ':', '@', '&', '?', '=', '/', '+', '$', ',', };
        internal static char[] RESERVED_COMPONENT = new char[] { '-', '.', '(', ')', ']', '*', '[', '_', '\'', '!', '~' };

        public Global_obj_encodeURI(Context context)
            : base(context)
        {
            SetOwnProperty("length", 1.0D);
        }

        public override object Call(IObj that, object[] args)
        {
            if(!HasArgs(args) || args[0] == null || args[0] is Undefined)
                throw new ArgumentException();

            var encoded = HttpUtility.UrlEncode(JsTypeConverter.ToString(args[0]));

            //TODO: these two loops will be manually unrolled for speed later
            foreach (char c in RESERVED)
            {
                var str = c.ToString();
                encoded = encoded.Replace(HttpUtility.UrlEncode(str), str);
            }

            foreach (char c in RESERVED_COMPONENT)
            {
                var str = c.ToString();
                encoded = encoded.Replace(HttpUtility.UrlEncode(str), str);
            }

            return encoded;
        }
    }
}
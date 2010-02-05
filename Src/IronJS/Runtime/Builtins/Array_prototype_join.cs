using System.Text;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using System;

namespace IronJS.Runtime.Builtins
{
    class Array_prototype_join : NativeFunction
    {
        public Array_prototype_join(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            var buf = new StringBuilder();
            var sep = HasArgN(args, 0) 
                         ? JsTypeConverter.ToString(args[0]) 
                         : ",";

            var length = (that is JsArray)
                ? (that as JsArray).Values.Length
                : JsTypeConverter.ToInt32(that.Get("length"));

            if (length == 0)
                return "";
            
            object value;
            for (var i = 0; i < length; ++i)
            {
                value = that.Get(i);

                buf.Append(sep);

                if (!(value is Undefined))
                    buf.Append(JsTypeConverter.ToString(value));
            }

            if (sep.Length > 0)
                return buf.ToString().Substring(sep.Length, buf.Length - sep.Length);

            return buf.ToString();
        }
    }
}

using System.Text;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

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
            // 15.4.4.5
            string sep;

            if (HasArgs(args))
            {
                if (args[0] is Undefined)
                {
                    sep = ",";
                }
                else
                {
                    sep = JsTypeConverter.ToString(args[0]);
                }
            }
            else
            {
                sep = ",";
            }


            var length = JsTypeConverter.ToInt32(that.Get("length"));

            if (length == 0)
                return "";

            var sb = new StringBuilder();

            for (var i = 0.0D; i < (double)length; ++i)
            {
                var val = that.Get(i);
                sb.Append(sep);

                if (!(val is Undefined))
                    sb.Append(JsTypeConverter.ToString(val));
            }

            if (sep.Length > 0)
                return sb.ToString().Substring(sep.Length, sb.Length - sep.Length);

            return sb.ToString();
        }
    }
}

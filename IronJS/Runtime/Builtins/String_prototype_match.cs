using System;
using System.Text.RegularExpressions;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class String_prototype_match : NativeFunction
    {
        public String_prototype_match(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            if (!HasArgs(args))
                throw new ArgumentException();

            var str = JsTypeConverter.ToString(that);
            var regex = JsTypeConverter.ToRegExp(args[0], Context);
            var global = JsTypeConverter.ToBoolean(regex.Get("global"));

            if (!global)
                return (regex.Get("exec") as IFunction).Call(regex, new[] { (object) that });

            var matches = regex.Match.Matches(str);
            var results = Context.ArrayConstructor.Construct();
            
            var d = 0.0D;
            foreach (Match match in matches)
                results.SetOwnProperty(d++, match.Groups[0].Value);

            return results;
        }
    }
}

using System;
using System.Text.RegularExpressions;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class RegExp_prototype_exec : NativeFunction
    {
        public RegExp_prototype_exec(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            if (!(that is RegExpObj))
                throw new ShouldThrowTypeError();

            if (!HasArgs(args))
                throw new ArgumentException();

            var regexp = that as RegExpObj;
            var str = JsTypeConverter.ToString(args[0]);
            var length = str.Length;
            var lastIndex = JsTypeConverter.ToInt32(that.Get("lastIndex"));
            var global = JsTypeConverter.ToBoolean(that.Get("global"));

            if (global)
                lastIndex = 0;

            if (lastIndex < 0 || lastIndex > length)
            {
                lastIndex = 0;
                return null;
            }

            var match = regexp.Match.Match(str, lastIndex);

            if (!match.Success)
            {
                lastIndex = 0;
                return null;
            }
            
            int n = match.Groups.Count;
            int e = match.Index + match.Length;

            if (global)
                lastIndex = e;

            var resultArray = Context.ArrayConstructor.Construct();
            resultArray.SetOwnProperty("index", match.Index);
            resultArray.SetOwnProperty("input", str);
            resultArray.SetOwnProperty("length", n);

            var d = 0.0D;
            foreach (Group group in match.Groups)
                resultArray.SetOwnProperty(d++, group.Value);

            return resultArray;
        }
    }
}

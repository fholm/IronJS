using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class String_prototype_search : NativeFunction
    {
        public String_prototype_search(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            if (!HasArgs(args))
                throw new ArgumentException();

            var target = JsTypeConverter.ToString(that);
            var regex = JsTypeConverter.ToRegExp(args[0], Context);
            var match = regex.Match.Match(target);

            if (match.Success)
                return (double) match.Index;

            return -1.0D;
        }
    }
}

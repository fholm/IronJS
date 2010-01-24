using System;
using System.Linq;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Microsoft.Scripting.Utils;

namespace IronJS.Runtime.Builtins
{
    class String_prototype_concat : NativeFunction
    {
        public String_prototype_concat(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            args = ArrayUtils.Insert((object)that, args);

            return String.Concat(
                args.Select(x => JsTypeConverter.ToString(x))
            );
        }

    }
}

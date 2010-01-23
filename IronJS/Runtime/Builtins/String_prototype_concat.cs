using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Js;
using Microsoft.Scripting.Utils;
using IronJS.Runtime.Utils;

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

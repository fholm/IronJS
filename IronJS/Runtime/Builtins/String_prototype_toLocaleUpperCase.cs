using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class String_prototype_toLocaleUpperCase : NativeFunction
    {
        public String_prototype_toLocaleUpperCase(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            return JsTypeConverter.ToString(that).ToUpper();
        }
    }
}

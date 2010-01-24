using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Js;

namespace IronJS.Runtime.Builtins
{
    class String_prototype_replace : NativeFunction
    {
        public String_prototype_replace(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            throw new NotImplementedException();
        }
    }
}

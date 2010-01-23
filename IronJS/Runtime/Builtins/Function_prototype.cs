using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Runtime.Builtins
{
    class Function_prototype : NativeFunction
    {
        public Function_prototype(Context context)
            : base(context, null)
        {

        }

        public override object Call(Js.IObj that, object[] args)
        {
            return Js.Undefined.Instance;
        }
    }
}

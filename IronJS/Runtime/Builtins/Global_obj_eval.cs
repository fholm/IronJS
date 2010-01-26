using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Js.Descriptors;

namespace IronJS.Runtime.Builtins
{
    class Global_obj_eval : NativeFunction
    {
        public Global_obj_eval(Context context)
            : base(context)
        {
            Set("length",
                new UserProperty(this, 1.0D)
            );
        }

        public override object Call(IObj that, object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
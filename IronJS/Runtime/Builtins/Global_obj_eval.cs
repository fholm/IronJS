using System;
using IronJS.Runtime.Js;

namespace IronJS.Runtime.Builtins
{
    class Global_obj_eval : NativeFunction
    {
        public Global_obj_eval(Context context)
            : base(context)
        {
            SetOwnProperty("length", 1.0D);
        }

        public override object Call(IObj that, object[] args)
        {
            throw new NotImplementedException();
        }
    }
}
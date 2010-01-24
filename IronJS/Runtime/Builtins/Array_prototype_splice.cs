using System;
using IronJS.Runtime.Js;

namespace IronJS.Runtime.Builtins
{
    class Array_prototype_splice : NativeFunction
    {
        public Array_prototype_splice(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            throw new NotImplementedException();
        }
    }
}

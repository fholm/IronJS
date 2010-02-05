using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using IronJS.Runtime.Js.Descriptors;

namespace IronJS.Runtime.Builtins
{
    class Math_obj_random : NativeFunction
    {
        Random rnd = new Random();

        public Math_obj_random(Context context)
            : base(context)
        {
            Set("length", new UserProperty(this, 0.0D));
        }

        public override object Call(IObj that, object[] args)
        {
            lock (rnd)
                return rnd.NextDouble();
        }
    }
}
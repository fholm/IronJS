using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class Math_obj_random : NativeFunction
    {
        Random rnd = new Random();

        public Math_obj_random(Context context)
            : base(context)
        {
            SetOwnProperty("length", 0.0D);
        }

        public override object Call(IObj that, object[] args)
        {
            lock (rnd)
                return rnd.NextDouble();
        }
    }
}
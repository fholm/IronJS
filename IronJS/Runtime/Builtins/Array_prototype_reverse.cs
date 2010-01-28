using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class Array_prototype_reverse : NativeFunction
    {
        public Array_prototype_reverse(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            if (that is JsArray)
            {
                Array.Reverse((that as JsArray).Values);
                return that;
            }

            throw new NotImplementedException();
        }
    }
}

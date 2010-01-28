using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Microsoft.Scripting.Utils;
using System;

namespace IronJS.Runtime.Builtins
{
    class Array_prototype_shift : NativeFunction
    {
        public Array_prototype_shift(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            if (that is JsArray)
            {
                var array = that as JsArray;
                var length = array.Values.Length;

                if (length > 0)
                {
                    var shifted = array.Get(0);
                    array.Values = ArrayUtils.RemoveFirst(array.Values);
                    return shifted;
                }
                else
                {
                    return Undefined.Instance;
                }
            }

            throw new NotImplementedException();
        }
    }
}

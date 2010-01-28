using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Microsoft.Scripting.Utils;
using System;

namespace IronJS.Runtime.Builtins
{
    class Array_prototype_pop : NativeFunction
    {
        public Array_prototype_pop(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            if (that is JsArray)
            {
                var array = that as JsArray;
                var length = array.Values.Length;

                if(length > 0)
                {
                    var poped = array.Get(length - 1);
                    array.Values = ArrayUtils.RemoveLast(array.Values);
                    return poped;
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

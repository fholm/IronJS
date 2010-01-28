using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using System;
using Microsoft.Scripting.Utils;

namespace IronJS.Runtime.Builtins
{
    class Array_prototype_push : NativeFunction
    {
        public Array_prototype_push(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            if (that is JsArray)
            {
                //TODO: optimize this to grow array once instead of multiple times
                var array = that as JsArray;
                var length = array.Values.Length;

                foreach (var arg in args)
                    array.Set(length++, arg);

                return length;
            }

            throw new NotImplementedException();
        }
    }
}

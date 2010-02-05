using System;
using IronJS.Runtime.Js;
using Microsoft.Scripting.Utils;
using IronJS.Runtime.Js.Descriptors;

namespace IronJS.Runtime.Builtins
{
    class Array_prototype_unshift : NativeFunction
    {
        public Array_prototype_unshift(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            if (that is JsArray)
            {
                //TODO: optimize this to grow array once instead of multiple times
                var array = that as JsArray;

                foreach (var arg in args)
                    array.Values = ArrayUtils.Insert(
                        new UserProperty(that, arg), 
                        array.Values
                    );

                return array;
            }

            throw new NotImplementedException();
        }
    }
}

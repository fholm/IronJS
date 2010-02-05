using System.Collections.Generic;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using System;

namespace IronJS.Runtime.Builtins
{
    class Array_prototype_sort : NativeFunction
    {
        public Array_prototype_sort(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            if (that is JsArray)
            {
                var array = that as JsArray;

                if (HasArgN(args, 0))
                {
                    var func = args[0] as IFunction;
                    if (func == null)
                        throw new ShouldThrowTypeError();

                    Array.Sort(array.Values, (a, b) =>
                        Convert.ToInt32(func.Call(that, new[] { a.Get(), b.Get() }))
                    );
                }
                else
                {
                    Array.Sort(array.Values, (a, b) =>
                          JsTypeConverter.ToInt32(JsTypeConverter.ToPrimitive(a.Get()))
                        - JsTypeConverter.ToInt32(JsTypeConverter.ToPrimitive(b.Get()))
                    );
                }

                return array;
            }

            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Js;

namespace IronJS.Runtime.Builtins
{
    static class ArrayObject
    {
        internal static IFunction CreateConstructor(Context context)
        {
            var obj = context.CreateFunction(
                context.BuiltinsFrame,
                new Lambda(
                    (that, frame) =>
                    {
                        return null;
                    }
                )
            );

            obj.SetOwnProperty("prototype", context.ArrayPrototype);
            obj.SetOwnProperty("length", 1D);

            return obj;
        }

        internal static IObj CreatePrototype(Context context)
        {
            var obj = context.CreateObject();

            obj.SetOwnProperty(
                "pop",
                context.CreateFunction(
                    context.BuiltinsFrame,
                    new Lambda(
                        (that, frame) =>
                        {
                            return null;
                        }
                    )
                )
            );

            return obj;
        }
    }
}

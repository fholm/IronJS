using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Js;

namespace IronJS.Runtime.Builtins
{
    static class FunctionObject
    {
        static internal IFunction CreateConstructor(Context context)
        {
            var obj = context.CreateFunction(
                context.BuiltinsFrame,
                new Lambda(
                    new Func<IObj, IFrame, object>((that, frame) => null),
                    new string[] { }.ToList()
                )
            );

            return obj;
        }

        static internal IFunction CreatePrototype(Context context)
        {
            var obj = context.CreateFunction(
                context.BuiltinsFrame,
                new Lambda(
                    new Func<IObj, IFrame, object>((that, frame) => Js.Undefined.Instance),
                    new string[] { }.ToList()
                )
            );

            return obj;
        }
    }
}

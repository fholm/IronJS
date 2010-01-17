using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Js;

namespace IronJS.Runtime.Builtins
{
    static class BooleanObject
    {
        static internal IObj CreatePrototype(Context context)
        {
            var obj = context.CreateObject();

            obj.Put("toString",
                context.CreateFunction(
                    context.SuperGlobals,
                    new Lambda((that, frame) =>
                    {
                        if (that.HasValue())
                            return (that as IValueObj).Value.ToString().ToLower();
                        throw new NotImplementedException();
                    })
                )
            );

            return obj;
        }
    }
}

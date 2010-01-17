using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Js;
using System.Globalization;

namespace IronJS.Runtime.Builtins
{
    static class NumberObject
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
                                return Convert.ToString(
                                    (double)((that as IValueObj).Value), 
                                    CultureInfo.InvariantCulture
                                 );

                            throw new NotImplementedException();
                        }
                    )
                )
            );

            return obj;
        }
    }
}

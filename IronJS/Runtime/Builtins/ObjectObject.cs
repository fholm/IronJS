using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Js;

namespace IronJS.Runtime.Builtins
{
    static class ObjectObject
    {
        static internal IFunction CreateConstructor(Context context)
        {
            var obj = context.CreateFunction(
                context.BuiltinsFrame,
                new Lambda(
                    new Func<IObj, IFrame, object>((that, frame) => null),
                    new[] { "value" }.ToList()
                )
            );

            return obj;
        }

        static internal IObj CreatePrototype(Context context)
        {
            var obj = new Obj();

            obj.Class = ObjClass.Object;
            obj.Prototype = null;
            obj.Context = context;

            return obj;
        }
    }
}

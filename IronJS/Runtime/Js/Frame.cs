using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;

namespace IronJS.Runtime.Js
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;

    public class Frame : Obj
    {
        public Frame(Context context)
            : this(null, context)
        {

        }

        public Frame(IObj prototype, Context context)
        {
            Class = ObjClass.Frame;
            Context = context;
            Prototype = prototype;
        }
    }
}

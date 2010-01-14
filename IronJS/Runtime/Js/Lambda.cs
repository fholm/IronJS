using System;
using System.Collections.Generic;
using System.Dynamic;

namespace IronJS.Runtime.Js
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;

    public class Lambda
    {
        internal readonly Func<IFrame, object> Delegate;
        internal readonly List<string> Params;

        public Lambda(Func<IFrame, object> func, List<string> parms)
        {
            Delegate = func;
            Params = parms;
        }
    }
}

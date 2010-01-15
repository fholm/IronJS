using System;
using System.Collections.Generic;
using System.Dynamic;

namespace IronJS.Runtime.Js
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;

    public class Lambda
    {
        public Func<IObj, IFrame, object> Delegate { get; protected set; }
        public List<string> Params { get; protected set; }

        public Lambda(Func<IObj, IFrame, object> func, List<string> parms)
        {
            Delegate = func;
            Params = parms;
        }
    }
}

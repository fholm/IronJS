using System;
using System.Collections.Generic;
using System.Dynamic;

namespace IronJS.Runtime.Js
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;

    public class Function
    {
        internal readonly Func<Frame, object> Lambda;
        internal readonly List<string> Params;

        public Function(Func<Frame, object> lambda, List<string> parms)
        {
            Lambda = lambda;
            Params = parms;
        }
    }
}

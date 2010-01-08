using System;
using System.Collections.Generic;
using System.Dynamic;

namespace IronJS.Runtime.Js
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;

    public class Function : IDynamicMetaObjectProvider
    {
        internal readonly Action<Frame> Lambda;
        internal readonly List<string> Params;

        internal Function(Action<Frame> lambda, List<string> parms)
        {
            Lambda = lambda;
            Params = parms;
        }
    
        #region IDynamicMetaObjectProvider Members

        Meta  IDynamicMetaObjectProvider.GetMetaObject(Et parameter)
        {
            return new FunctionMeta(parameter, this);
        }

        #endregion
    }
}

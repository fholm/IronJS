
namespace IronJS.Runtime.Js
{
    using System;
    using System.Collections.Generic;
    using System.Dynamic;
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;

    public class Function : IDynamicMetaObjectProvider
    {
        internal readonly Delegate Lambda;
        internal readonly List<string> Params;

        internal Function(Delegate func, List<string> parms)
        {
            Lambda = func;
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

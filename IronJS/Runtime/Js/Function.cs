using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Runtime.Js
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;
    using Restrict = System.Dynamic.BindingRestrictions;
    using AstUtils = Microsoft.Scripting.Ast.Utils;

    using System;
    using System.Dynamic;
    using System.Collections.Generic;

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

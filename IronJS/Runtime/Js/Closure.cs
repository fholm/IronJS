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
    using IronJS.Runtime.Utils;

    class Closure : IDynamicMetaObjectProvider
    {
        public readonly Frame<Function> Table;
        public readonly Frame<object> Frame;
        public readonly Function Function;

        public Closure(Frame<Function> table, Frame<object> frame, Function function)
        {
            Table = table;
            Frame = frame;
            Function = function;
        }

        #region IDynamicMetaObjectProvider Members

        Meta IDynamicMetaObjectProvider.GetMetaObject(Et parameter)
        {
            return new ClosureMeta(parameter, this);
        }

        #endregion
    }
}

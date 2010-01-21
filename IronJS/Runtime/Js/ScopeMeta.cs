using System.Dynamic;
using IronJS.Runtime.Utils;
using Microsoft.Scripting.Utils;
using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;
using Restrict = System.Dynamic.BindingRestrictions;

namespace IronJS.Runtime.Js
{
    class ScopeMeta : Meta
    {
        public ScopeMeta(Et expression, Scope scope)
            : base(expression, Restrict.Empty, scope)
        {

        }



    }
}

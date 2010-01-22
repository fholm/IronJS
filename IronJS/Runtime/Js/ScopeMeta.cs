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
        public ScopeMeta(Et parameter, Scope scope)
            : base(parameter, Restrict.Empty, scope)
        {

        }

        public override Meta BindInvokeMember(InvokeMemberBinder binder, Meta[] args)
        {
            return null;
        }
    }
}

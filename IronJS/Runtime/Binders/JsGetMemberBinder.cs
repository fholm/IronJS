using System;
using System.Dynamic;

namespace IronJS.Runtime.Binders
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;
    using Restrict = System.Dynamic.BindingRestrictions;

    class JsGetMemberBinder : GetMemberBinder
    {
        public JsGetMemberBinder(object name)
            : base(name.ToString(), false)
        {

        }

        public override Meta FallbackGetMember(Meta target, Meta error)
        {
            throw new NotImplementedException();
        }
    }
}

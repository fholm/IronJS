using System;
using System.Dynamic;

namespace IronJS.Runtime.Binders
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;
    using Restrict = System.Dynamic.BindingRestrictions;

    class JsGetMemberBinder : GetMemberBinder
    {
        Context _context;

        public JsGetMemberBinder(object name, Context context)
            : base(name.ToString(), false)
        {
            _context = context;
        }

        public override Meta FallbackGetMember(Meta target, Meta error)
        {
            throw new NotImplementedException();
        }
    }
}

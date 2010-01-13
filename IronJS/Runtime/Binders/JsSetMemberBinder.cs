using System;
using System.Dynamic;

namespace IronJS.Runtime.Binders
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;
    using Restrict = System.Dynamic.BindingRestrictions;

    class JsSetMemberBinder : SetMemberBinder
    {
        Context _context;
        internal readonly Js.PropertyAttrs Attrs;

        public JsSetMemberBinder(object name, Js.PropertyAttrs attrs, Context context)
            : base(name.ToString(), false)
        {
            Attrs = attrs;
            _context = context;
        }

        public JsSetMemberBinder(object name, Context context)
            : this(name, 0, context)
        {

        }

        public override Meta FallbackSetMember(Meta target, Meta value, Meta error)
        {
            throw new NotImplementedException();
        }
    }
}

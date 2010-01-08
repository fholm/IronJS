using System;
using System.Dynamic;

namespace IronJS.Runtime.Binders
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;
    using Restrict = System.Dynamic.BindingRestrictions;

    class JsSetMemberBinder : SetMemberBinder
    {
        internal readonly Js.PropertyAttrs Attrs;

        public JsSetMemberBinder(object name, Js.PropertyAttrs attrs)
            : base(name.ToString(), false)
        {
            Attrs = attrs;
        }

        public JsSetMemberBinder(object name)
            : this(name, 0)
        {

        }

        public override Meta FallbackSetMember(Meta target, Meta value, Meta error)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Dynamic;

namespace IronJS.Runtime.Binders
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;
    using Restrict = System.Dynamic.BindingRestrictions;

    class JsInvokeMemberBinder : InvokeMemberBinder
    {
        public JsInvokeMemberBinder(object name, CallInfo callInfo)
            : base(name.ToString(), false, callInfo)
        {

        }

        public override Meta FallbackInvoke(Meta target, Meta[] args, Meta error)
        {
            throw new NotImplementedException();
        }

        public override Meta FallbackInvokeMember(Meta target, Meta[] args, Meta error)
        {
            throw new NotImplementedException();
        }
    }
}

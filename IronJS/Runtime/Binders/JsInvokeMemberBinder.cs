using System;
using System.Dynamic;

namespace IronJS.Runtime.Binders
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;
    using Restrict = System.Dynamic.BindingRestrictions;

    class JsInvokeMemberBinder : InvokeMemberBinder
    {
        Context _context;

        public JsInvokeMemberBinder(object name, CallInfo callInfo, Context context)
            : base(name.ToString(), false, callInfo)
        {
            _context = context;
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

using System;
using System.Dynamic;
using IronJS.Runtime.Utils;
using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Binders
{
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
            Meta[] deferArgs;
            if (BinderUtils.NeedsToDefer(target, args, out deferArgs))
                return Defer(deferArgs);

            throw new NotImplementedException();
        }

        public override Meta FallbackInvokeMember(Meta target, Meta[] args, Meta error)
        {
            Meta[] deferArgs;
            if (BinderUtils.NeedsToDefer(target, args, out deferArgs))
                return Defer(deferArgs);

            throw new NotImplementedException();
        }
    }
}

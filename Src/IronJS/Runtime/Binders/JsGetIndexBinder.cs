using System;
using System.Dynamic;
using IronJS.Runtime.Utils;
using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Binders
{
    class JsGetIndexBinder : GetIndexBinder
    {
        Context _context;

        public JsGetIndexBinder(CallInfo callInfo, Context context)
            : base(callInfo)
        {
            _context = context;
        }

        public override Meta FallbackGetIndex(Meta target, Meta[] indexes, Meta error)
        {
            Meta[] deferArgs;
            if (BinderUtils.NeedsToDefer(target, indexes, out deferArgs))
                return Defer(deferArgs);

            throw new NotImplementedException();
        }
    }
}

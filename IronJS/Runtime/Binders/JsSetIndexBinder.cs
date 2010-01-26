using System;
using System.Dynamic;
using IronJS.Runtime.Utils;
using Microsoft.Scripting.Utils;
using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Binders
{
    class JsSetIndexBinder : SetIndexBinder
    {
        Context _context;

        public JsSetIndexBinder(CallInfo callInfo, Context context)
            : base(callInfo)
        {
            _context = context;
        }

        public override Meta FallbackSetIndex(Meta target, Meta[] indexes, Meta value, Meta error)
        {
            Meta[] deferArgs;
            if (BinderUtils.NeedsToDefer(target, ArrayUtils.Insert(value, indexes), out deferArgs))
                return Defer(deferArgs);

            throw new NotImplementedException();
        }
    }
}

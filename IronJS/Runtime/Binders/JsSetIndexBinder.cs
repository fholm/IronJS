using System;
using System.Dynamic;
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
            throw new NotImplementedException();
        }
    }
}

using System;
using System.Dynamic;
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
            throw new NotImplementedException();
        }
    }
}

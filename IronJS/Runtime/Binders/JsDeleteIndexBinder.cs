using System;
using System.Dynamic;
using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Binders
{
    class JsDeleteIndexBinder : DeleteIndexBinder
    {
        Context _context;

        public JsDeleteIndexBinder(CallInfo callInfo, Context context)
            : base(callInfo)
        {
            _context = context;
        }

        public override Meta FallbackDeleteIndex(Meta target, Meta[] indexes, Meta errorSuggestion)
        {
            throw new NotImplementedException();
        }
    }
}

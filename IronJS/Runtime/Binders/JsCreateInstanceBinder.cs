using System;
using System.Dynamic;
using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Binders
{
    class JsCreateInstanceBinder : CreateInstanceBinder
    {
        Context _context;

        public JsCreateInstanceBinder(CallInfo callInfo, Context context)
            : base(callInfo)
        {
            _context = context;
        }

        public override Meta FallbackCreateInstance(Meta target, Meta[] args, Meta errorSuggestion)
        {
            throw new NotImplementedException();
        }
    }
}

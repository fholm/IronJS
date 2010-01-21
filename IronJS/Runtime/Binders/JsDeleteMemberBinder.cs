using System;
using System.Dynamic;
using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Binders
{
    class JsDeleteMemberBinder : DeleteMemberBinder
    {
        Context _context;

        public JsDeleteMemberBinder(object name, Context context)
            : base(name.ToString(), false)
        {
            _context = context;
        }

        public override Meta FallbackDeleteMember(Meta target, Meta errorSuggestion)
        {
            throw new NotImplementedException();
        }
    }
}

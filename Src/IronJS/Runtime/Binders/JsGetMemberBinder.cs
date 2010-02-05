using System;
using System.Dynamic;
using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Binders
{
    class JsGetMemberBinder : GetMemberBinder
    {
        Context _context;

        public JsGetMemberBinder(object name, Context context)
            : base(name.ToString(), false)
        {
            _context = context;
        }

        public override Meta FallbackGetMember(Meta target, Meta error)
        {
            if (!target.HasValue)
                return Defer(target);

            throw new NotImplementedException();
        }
    }
}

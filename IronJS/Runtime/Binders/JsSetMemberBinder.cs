using System;
using System.Dynamic;
using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Binders
{
    class JsSetMemberBinder : SetMemberBinder
    {
        Context _context;

        public JsSetMemberBinder(object name, Context context)
            : base(name.ToString(), false)
        {
            _context = context;
        }

        public override Meta FallbackSetMember(Meta target, Meta value, Meta error)
        {
            throw new NotImplementedException();
        }
    }
}

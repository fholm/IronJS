using System.Dynamic;

namespace IronJS.Runtime2.Binders
{
    public class IjsInvokeBinder : InvokeBinder
    {
        public IjsInvokeBinder(CallInfo callInfo)
            : base(callInfo)
        {

        }

        public override DynamicMetaObject FallbackInvoke(DynamicMetaObject target, DynamicMetaObject[] args, DynamicMetaObject errorSuggestion)
        {
            throw new System.NotImplementedException();
        }
    }
}

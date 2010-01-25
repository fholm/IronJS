using Microsoft.Scripting.Utils;
using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Utils
{
    static class BinderUtils
    {
        static public bool NeedsToDefer(Meta target, Meta[] args, out Meta[] result)
        {
            if (!target.HasValue)
                goto Defer;

            for (int i = 0; i < args.Length; ++i)
            {
                if (!args[i].HasValue)
                    goto Defer;
            }

            result = null;
            return false;

            Defer:
                result = ArrayUtils.Insert(target, args);
                return true;
        }
    }
}

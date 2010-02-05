using Meta = System.Dynamic.DynamicMetaObject;
using Restrict = System.Dynamic.BindingRestrictions;

namespace IronJS.Runtime.Utils
{
    enum RestrictFlag { Instance, Type }

    static class RestrictUtils
    {
        internal static Restrict BuildCallRestrictions(Meta target, Meta[] args, RestrictFlag flag)
        {
            var restrictions =
                target.Restrictions.Merge(
                    Restrict.Combine(args)
                );

            if (flag == RestrictFlag.Instance)
            {
                restrictions =
                    restrictions.Merge(
                        Restrict.GetInstanceRestriction(
                            target.Expression,
                            target.Value
                        )
                    );
            }
            else
            {
                restrictions =
                    restrictions.Merge(
                        Restrict.GetTypeRestriction(
                            target.Expression,
                            target.LimitType
                        )
                    );
            }

            for (var i = 0; i < args.Length; ++i)
            {
                Restrict restr;

                // HasValue and Value == null, means we have a null value
                if (args[i].HasValue && args[i].Value == null)
                {
                    restr =
                        Restrict.GetInstanceRestriction(
                            args[i].Expression, 
                            null
                        );
                }
                else
                {
                    restr =
                        Restrict.GetTypeRestriction(
                            args[i].Expression, 
                            args[i].LimitType
                        );
                }

                restrictions = restrictions.Merge(restr);
            }

            return restrictions;
        }

        internal static Restrict GetNullHandledTypeRestriction(Meta target)
        {
            if (target.HasValue && target.Value == null)
                return Restrict.GetInstanceRestriction(
                        target.Expression,
                        null
                    );

            return Restrict.GetTypeRestriction(
                    target.Expression,
                    target.LimitType
                );
        }
    }
}

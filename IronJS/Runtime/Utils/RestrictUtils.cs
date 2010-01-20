
namespace IronJS.Runtime.Utils
{
    using Meta2 = System.Dynamic.DynamicMetaObject;
    using Restrict2 = System.Dynamic.BindingRestrictions;

    enum RestrictFlag { Instance, Type }

    static class RestrictUtils
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="target"></param>
        /// <param name="args"></param>
        /// <param name="instanceRestrict"></param>
        /// <returns></returns>
        internal static Restrict2 BuildCallRestrictions(Meta2 target, Meta2[] args, RestrictFlag flag)
        {
            var restrictions =
                target.Restrictions.Merge(
                    Restrict2.Combine(args)
                );

            if (flag == RestrictFlag.Instance)
            {
                restrictions =
                    restrictions.Merge(
                        Restrict2.GetInstanceRestriction(
                            target.Expression,
                            target.Value
                        )
                    );
            }
            else
            {
                restrictions =
                    restrictions.Merge(
                        Restrict2.GetTypeRestriction(
                            target.Expression,
                            target.LimitType
                        )
                    );
            }

            for (var i = 0; i < args.Length; ++i)
            {
                Restrict2 restr;

                // HasValue and Value == null, means we have a null value
                if (args[i].HasValue && args[i].Value == null)
                {
                    restr =
                        Restrict2.GetInstanceRestriction(
                            args[i].Expression, 
                            null
                        );
                }
                else
                {
                    restr =
                        Restrict2.GetTypeRestriction(
                            args[i].Expression, 
                            args[i].LimitType
                        );
                }

                restrictions = restrictions.Merge(restr);
            }

            return restrictions;
        }

        internal static Restrict2 GetNullHandledTypeRestriction(Meta2 target)
        {
            if (target.HasValue && target.Value == null)
                return Restrict2.GetInstanceRestriction(
                        target.Expression,
                        null
                    );

            return Restrict2.GetTypeRestriction(
                    target.Expression,
                    target.LimitType
                );
        }
    }
}

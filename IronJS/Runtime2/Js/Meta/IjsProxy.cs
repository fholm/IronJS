using System.Dynamic;
using System.Linq;
using Microsoft.Scripting.Utils;
using Binding = System.Dynamic.BindingRestrictions;
using MetaObj = System.Dynamic.DynamicMetaObject;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Runtime2.Js.Meta
{
    public class IjsProxy : IjsMeta<Js.IjsProxy>
    {
        public IjsProxy(Et parameter, Js.IjsProxy ijsProxy)
            : base(parameter, ijsProxy)
        {

        }

        public override MetaObj BindInvoke(InvokeBinder binder, MetaObj[] args)
        {
            var lambda = Self.Node.Compile(
                Self.ClosureType,
                args.Select(x => x.LimitType).ToArray()
            );

            return new MetaObj(
                Et.Invoke(
                    Et.Constant(lambda, lambda.GetType()),
                    ArrayUtils.Insert<Et>(
                        Et.Constant(Self.Closure, Self.ClosureType),
                        args.Select(x => Et.Convert(x.Expression, x.LimitType)).ToArray()
                    )
                ),
                CreateRestriction(args)
            );
        }

        Binding CreateRestriction(MetaObj[] args)
        {
            var restrictions =
                Binding.GetTypeRestriction(
                    Et.Field(
                        SelfExpr, "Closure"
                    ),
                    Self.ClosureType
                );

            foreach(var arg in args)
            {
                if (arg.HasValue && arg.Value == null)
                {
                    restrictions = restrictions.Merge(
                        Binding.GetInstanceRestriction(
                            arg.Expression,
                            null
                        )
                    );
                }
                else
                {
                    restrictions = restrictions.Merge(
                        Binding.GetTypeRestriction(
                            arg.Expression,
                            arg.LimitType
                        )
                    );
                }
            }

            return restrictions;
        }
    }
}

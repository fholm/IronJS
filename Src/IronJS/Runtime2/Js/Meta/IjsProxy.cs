using System;
using System.Dynamic;

using IronJS.Tools;
using Microsoft.Scripting.Utils;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif


namespace IronJS.Runtime2.Js.Meta
{
    using Et = Expression;
    using Binding = BindingRestrictions;
    using MetaObj = DynamicMetaObject;

    public class IjsProxy : IjsMeta<Js.IjsFunc>
    {
        public IjsProxy(Et parameter, Js.IjsFunc ijsProxy)
            : base(parameter, ijsProxy)
        {

        }

        public override MetaObj BindInvoke(InvokeBinder binder, MetaObj[] args)
        {
            throw new NotImplementedException();
        }

        Binding CreateRestriction(MetaObj[] args)
        {
            Binding restrictions =
                Binding.GetTypeRestriction(
                    Et.Field(
                        SelfExpr, "Closure"
                    ),
                    Self.ClosureType
                );

            foreach (MetaObj arg in args)
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

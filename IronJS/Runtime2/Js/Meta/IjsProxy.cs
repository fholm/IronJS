using System;
using System.Dynamic;
using System.Linq;
using IronJS.Extensions;
using Microsoft.Scripting.Utils;
using Binding = System.Dynamic.BindingRestrictions;
using Et = System.Linq.Expressions.Expression;
using MetaObj = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime2.Js.Meta
{
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

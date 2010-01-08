using System;
using System.Collections.Generic;
using System.Dynamic;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Js
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;
    using Restrict = System.Dynamic.BindingRestrictions;

    class ClosureMeta : Meta
    {
        public ClosureMeta(Et parameter, Closure closure)
            : base(parameter, Restrict.Empty, closure)
        {

        }

        public override Meta BindInvoke(InvokeBinder binder, Meta[] args)
        {
            var selfExpr = EtUtils.Cast<Closure>(this.Expression);
            var selfValue = (Closure)this.Value;

            var func = selfValue.Function;
            var invk = func.Lambda.GetType().GetMethod("Invoke");
            var functionField = EtUtils.Cast<Function>(Et.Field(selfExpr, "Function"));

            var target = Et.Field(functionField, "Lambda");
            var callFrame = Et.Parameter(typeof(Frame), "#callframe");
            var closureFrame = Et.Field(selfExpr, "Frame");

            var exprs = new List<Et>();

            exprs.Add(Frame.Enter(callFrame, closureFrame));

            for (int i = 1; i < args.Length; ++i)
            {
                exprs.Add(
                    Frame.Var(
                        callFrame,
                        func.Params[i - 1],
                        args[i].Expression,
                        VarType.Local
                    )
                );
            }

            exprs.Add(
                EtUtils.Box(Et.Call(
                    Et.Convert(target, func.Lambda.GetType()),
                    invk,
                    EtUtils.Cast<Frame>(callFrame)
                ))
            );

            return new Meta(
                Et.Block(
                    new[] { callFrame },
                    exprs
                ),
                Restrict.GetInstanceRestriction(
                    target, 
                    func.Lambda
                ).Merge(
                    RestrictUtils.BuildCallRestrictions(
                        this,
                        args,
                        RestrictFlag.Type
                    )
                )
            );
            
        }
    }
}

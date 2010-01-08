using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;

namespace IronJS.Runtime.Js
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;
    using Restrict = System.Dynamic.BindingRestrictions;
    using AstUtils = Microsoft.Scripting.Ast.Utils;

    using System;
    using System.Dynamic;
    using System.Collections.Generic;
    using IronJS.Runtime.Utils;

    class ClosureMeta : Meta
    {
        public ClosureMeta(Et parameter, Closure closure)
            : base(parameter, Restrict.Empty, closure)
        {

        }

        public override Meta BindInvoke(InvokeBinder binder, Meta[] args)
        {
            var self = EtUtils.Cast<Closure>(this.Expression);
            var closure = (Closure)this.Value;
            var func = closure.Function;

            var invk = func.Lambda.GetType().GetMethod("Invoke");
            var functionField = EtUtils.Cast<Function>(Et.Field(self, "Function"));

            var closureFrame = Et.Field(self, "Frame");
            var closureTable = Et.Field(self, "Table");
            var callFrame = Et.Parameter(typeof(Frame<object>), "#callframe");

            var target = Et.Field(functionField, "Lambda");
            var exprs = new List<Et>();

            exprs.Add(Frame<object>.Create(callFrame, closureFrame));

            for (int i = 2; i < args.Length; ++i)
            {
                exprs.Add(
                    Frame<object>.Var(
                        callFrame,
                        func.Params[i - 2],
                        args[i].Expression
                    )
                );
            }

            exprs.Add(
                EtUtils.Box(Et.Call(
                    Et.Convert(target, func.Lambda.GetType()),
                    invk,
                    EtUtils.Cast<Frame<Function>>(closureTable),
                    EtUtils.Cast<Frame<object>>(callFrame)
                ))
            );

            return new Meta(
                Et.Block(
                    new[] { callFrame },
                    exprs
                ),
                RestrictUtils.BuildCallRestrictions(
                    this,
                    args,
                    RestrictFlag.Instance
                )
            );
        }
    }
}

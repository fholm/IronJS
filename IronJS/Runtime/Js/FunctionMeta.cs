using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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

    class FunctionMeta : DynamicMetaObject
    {
        public FunctionMeta(System.Linq.Expressions.Expression parameter, object value)
            : base(parameter, Restrict.Empty, value)
        {

        }

        public override Meta BindInvoke(InvokeBinder binder, Meta[] args)
        {
            var func = (Function)this.Value;
            var invk = func.Lambda.GetType().GetMethod("Invoke");

            var callScope = Et.Parameter(typeof(Frame<object>), "#scopedyn");
            var target = Et.Field(EtUtils.Cast<Function>(this.Expression), "Lambda");
            var exprs = new List<Et>();

            exprs.Add(Frame<object>.Create(callScope, args[1].Expression));

            for (int i = 2; i < args.Length; ++i)
            {
                exprs.Add(
                    Frame<object>.Var(
                        callScope,
                        func.Params[i - 2],
                        args[i].Expression
                    )
                );
            }

            exprs.Add(
                EtUtils.Box(Et.Call(
                    Et.Convert(target, func.Lambda.GetType()),
                    invk,
                    EtUtils.Cast<Frame<Function>>(args[0].Expression),
                    EtUtils.Cast<Frame<object>>(callScope)
                ))
            );

            return new Meta(
                Et.Block(
                    new[] { callScope },
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

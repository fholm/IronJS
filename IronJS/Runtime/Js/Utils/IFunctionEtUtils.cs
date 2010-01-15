using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using IronJS.Runtime.Binders;
using IronJS.Runtime.Utils;
using Microsoft.Scripting.Utils;

namespace IronJS.Runtime.Js.Utils
{
    using Et = System.Linq.Expressions.Expression;
    using Parm = System.Linq.Expressions.ParameterExpression;
    using Meta = System.Dynamic.DynamicMetaObject;
    using Restrict = System.Dynamic.BindingRestrictions;
    using AstUtils = Microsoft.Scripting.Ast.Utils;

    static class IFunctionEtUtils
    {
        public static Et Frame(Et obj)
        {
            return Et.Property(
                EtUtils.Cast<IFunction>(obj),
                "Frame"
            );
        }

        public static Et Lambda(Et obj)
        {
            return Et.Property(
                EtUtils.Cast<IFunction>(obj),
                "Lambda"
            );
        }

        public static Et Delegate(Et obj)
        {
            return Et.Field(
                Lambda(obj),
                "Delegate"
            );
        }

        public static Et Call(Et obj, Et that, Et frame)
        {
            return Et.Call(
                Delegate(obj),
                typeof(Func<IObj, IFrame, object>).GetMethod("Invoke"),
                that,
                frame
            );
        }

        static internal Restrict BuildLambdaCallRestriction(Meta obj, Meta[] args)
        {
            return
                RestrictUtils.BuildCallRestrictions(
                    obj,
                    args,
                    RestrictFlag.Type
                ).Merge(
                    Restrict.GetInstanceRestriction(
                        IFunctionEtUtils.Lambda(obj.Expression),
                        (obj.Value as IFunction).Lambda
                    )
                );
        }

        internal static Et SetupCallBlock(Parm callFrame, Et selfExpr, IFunction selfObj, Meta[] args)
        {
            var argsObj = Et.Variable(typeof(IObj), "#arguments");

            return Et.Block(
                new[] { argsObj },

                // create a new empty call frame
                IFrameEtUtils.Enter(
                    callFrame,
                    IFunctionEtUtils.Frame(selfExpr)
                ),

                // create our new 'arguments' object
                Et.Assign(
                    argsObj,
                    Et.Call(
                        selfObj.ContextExpr(),
                        Context.Methods.CreateObject
                    )
                ),

                // block that setups our call frame + arguments objects
                IFunctionEtUtils.BuildCallFrame(
                    callFrame,
                    argsObj,
                    selfObj.Lambda.Params,
                    args
                )
            );
        }

        static internal Et BuildCallFrame(Parm callFrame, Parm argsObj, List<string> paramNames, Meta[] args)
        {
            return BuildCallFrame(callFrame, argsObj, paramNames, DynamicUtils.GetExpressions(args));
        }

        static internal Et BuildCallFrame(Parm callFrame, Parm argsObj, List<string> paramNames, Et[] args)
        {
            var exprs = new List<Et>();

            exprs.Add(
                IObjEtUtils.SetOwnProperty(
                    argsObj,
                    "length",
                    EtUtils.Box(Et.Constant(args.Length))
                )
            );

            exprs.Add(
                IFrameEtUtils.Push(
                    callFrame,
                    "arguments",
                    argsObj,
                    VarType.Local
                )
            );

            for (int i = 0; i < args.Length; ++i)
            {
                // only args with param names
                // should be pushed on the frame
                if (i < paramNames.Count)
                {
                    exprs.Add(
                        IFrameEtUtils.Push(
                            callFrame,
                            paramNames[i],
                            EtUtils.Box(args[i]),
                            VarType.Local
                        )
                    );
                }

                // push args on 'arguments'-object
                exprs.Add(
                    IObjEtUtils.SetOwnProperty(
                        argsObj,
                        i,
                        EtUtils.Box(args[i])
                    )
                );
            }

            return Et.Block(exprs);
        }
    }
}

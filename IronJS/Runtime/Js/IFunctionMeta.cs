using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using IronJS.Runtime.Binders;
using IronJS.Runtime.Utils;
using Microsoft.Scripting.Utils;

namespace IronJS.Runtime.Js
{
    using Et = System.Linq.Expressions.Expression;
    using Parm = System.Linq.Expressions.ParameterExpression;
    using Meta = System.Dynamic.DynamicMetaObject;
    using Restrict = System.Dynamic.BindingRestrictions;
    using AstUtils = Microsoft.Scripting.Ast.Utils;

    class IFunctionMeta : IObjMeta
    {
        static MethodInfo LambdaInvoke =
            typeof(Func<IFrame, object>).GetMethod("Invoke");

        public IFunctionMeta(Et parameter, IFunction function)
            : base(parameter, function)
        {

        }

        public override Meta BindCreateInstance(CreateInstanceBinder binder, Meta[] args)
        {
            var jsBinder = (JsCreateInstanceBinder)binder;
            var selfExpr = EtUtils.Cast<IFunction>(this.Expression);
            var selfObj = (IFunction)this.Value;
            var tmp = Et.Variable(typeof(IObj), "#tmp");

            var callFrame = Et.Variable(typeof(IFrame), "#callframe");
            var closureFrame = IFunctionUtils.Frame(selfExpr);
            var argsObj = Et.Variable(typeof(IObj), "#arguments");
            var exprs = new List<Et>();

            exprs.Add(
                FrameUtils.Push(
                    callFrame,
                    "this",
                    tmp,
                    VarType.Local
                )
            );

            PushArgsOnFrame(
                exprs,
                callFrame, 
                argsObj, 
                selfObj.Lambda, 
                args
            );

            return new Meta(
                Et.Block(
                    new[] { tmp, callFrame, argsObj },
                    // create our new object
                    Et.Assign(
                        tmp,
                        Et.Call(
                            selfObj.ContextExpr(),
                            Context.Methods.CreateObjectCtor,
                            selfExpr
                        )
                    ),
                    // create a new empty call frame
                    FrameUtils.Enter(
                        callFrame,
                        closureFrame
                    ),
                    // create our new 'arguments' object
                    Et.Assign(
                        argsObj,
                        Et.Call(
                            selfObj.ContextExpr(),
                            Context.Methods.CreateObject
                        )
                    ),
                    // block that setups our frame + arguments objects
                    Et.Block(
                        exprs
                    ),
                    // the actual constructor call
                    IFunctionUtils.Call(
                        this.Expression,
                        callFrame
                    ),
                    tmp
                ),
                RestrictUtils.BuildCallRestrictions(
                    this,
                    args,
                    RestrictFlag.Type
                ).Merge(
                    Restrict.GetInstanceRestriction(
                        IFunctionUtils.Lambda(selfExpr),
                        selfObj.Lambda
                    )
                )
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public override Meta BindInvoke(InvokeBinder binder, Meta[] args)
        {
            //TODO: insert defer

            if (binder is JsInvokeBinder)
            {
                var jsBinder = (JsInvokeBinder)binder;
                var selfExpr = EtUtils.Cast<IFunction>(this.Expression);
                var selfObj = (IFunction)this.Value;

                // this is the target delegate 
                // we're going to call
                var callTargetExpr = Et.Field(
                    EtUtils.Cast<Lambda>(
                        Et.Property(
                            selfExpr,
                            "Lambda"
                        )
                    ),
                    "Delegate"
                );

                // this is our new call frame
                var callFrameExpr = Et.Parameter(
                    typeof(IFrame),
                    "#callframe"
                );

                // argumentsExpr
                var argumentsExpr = Et.Parameter(
                    typeof(Obj),
                    "#arguments"
                );

                var exprs = new List<Et>();

                // this creates a new call frame
                // with the current one as parent
                exprs.Add(
                    FrameUtils.Enter(
                        callFrameExpr,
                        Et.Property(
                            selfExpr,
                            "Frame"
                        )
                    )
                );

                // create new 'arguments'-object
                exprs.Add(
                    Et.Assign(
                        argumentsExpr,
                        ObjUtils.CreateNew()
                    )
                );

                // add the 'callee' property to the
                // 'arguments'-object
                exprs.Add(
                    ObjUtils.SetOwnProperty(
                        argumentsExpr,
                        "callee",
                        this.Expression
                    )
                );

                // add the 'arguments' parameter
                // to the callframe
                exprs.Add(
                    FrameUtils.Push(
                        callFrameExpr,
                        "arguments",
                        argumentsExpr,
                        VarType.Local
                    )
                );

                var exprTree = 
                    SetupCallFrame(
                        exprs,
                        callTargetExpr,
                        callFrameExpr,
                        argumentsExpr,
                        selfObj.Lambda,
                        args,
                        Et.Default(typeof(object))
                    );

                return new Meta(
                    // exprs
                    exprTree,

                    // standard call restriction, on type
                    RestrictUtils.BuildCallRestrictions(
                        this,
                        args,
                        RestrictFlag.Type
                    ).Merge(
                    // additional restriction to
                    // to enforce this exact lambda
                        Restrict.GetInstanceRestriction(
                            Et.Property(
                                selfExpr,
                                "Lambda"
                            ),
                            selfObj.Lambda
                        )
                    )
                );
            }

            return binder.FallbackInvoke(this, args);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exprs"></param>
        /// <param name="callTargetExpr"></param>
        /// <param name="callFrameExpr"></param>
        /// <param name="argumentsExpr"></param>
        /// <param name="lambda"></param>
        /// <param name="args"></param>
        /// <param name="that"></param>
        /// <returns></returns>
        private Et SetupCallFrame(List<Et> exprs, Et callTargetExpr, Parm callFrameExpr, Parm argumentsExpr, Lambda lambda, Meta[] args, Et that)
        {
            PushArgsOnFrame(exprs, callFrameExpr, argumentsExpr, lambda, args);

            // hidden 'this' parameter
            exprs.Add(
                FrameUtils.Push(
                    callFrameExpr,
                    "this",
                    that,
                    VarType.Local
                )
            );

            // finally, emit the call
            exprs.Add(
                EtUtils.Box(
                    Et.Call(
                        callTargetExpr,
                        LambdaInvoke,
                        callFrameExpr
                    )
                )
            );

            return Et.Block(
                new[] { callFrameExpr, argumentsExpr },
                exprs
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="exprs"></param>
        /// <param name="callFrameExpr"></param>
        /// <param name="argumentsExpr"></param>
        /// <param name="lambda"></param>
        /// <param name="args"></param>
        private void PushArgsOnFrame(List<Et> exprs, Parm callFrameExpr, Parm argumentsExpr, Lambda lambda, Meta[] args)
        {
            exprs.Add(
                ObjUtils.SetOwnProperty(
                    argumentsExpr,
                    "length",
                    EtUtils.Box(Et.Constant(args.Length))
                )
            );

            exprs.Add(
                FrameUtils.Push(
                    callFrameExpr,
                    "arguments",
                    argumentsExpr,
                    VarType.Local
                )
            );

            for (int i = 0; i < args.Length; ++i)
            {
                // only args with param names
                // should be pushed on the frame
                if (i < lambda.Params.Count)
                {
                    exprs.Add(
                        FrameUtils.Push(
                            callFrameExpr,
                            lambda.Params[i],
                            EtUtils.Box(args[i].Expression),
                            VarType.Local
                        )
                    );
                }

                // push args on 'arguments'-object
                exprs.Add(
                    ObjUtils.SetOwnProperty(
                        argumentsExpr,
                        i,
                        EtUtils.Box(args[i].Expression)
                    )
                );
            }
        }
    }
}

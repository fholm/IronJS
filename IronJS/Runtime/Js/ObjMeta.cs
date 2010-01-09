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

    class ObjMeta : Meta
    {
        static MethodInfo LambdaInvoke = 
            typeof(Func<Frame, object>).GetMethod("Invoke");

        public ObjMeta(Et parameter, Obj closure)
            : base(parameter, Restrict.Empty, closure)
        {

        }

        public override Meta BindGetMember(GetMemberBinder binder)
        {
            //TODO: insert defer

            return new Meta(
                Et.Call(
                    EtUtils.Cast<Obj>(this.Expression),
                    typeof(Obj).GetMethod("Get"),
                    Et.Constant(binder.Name)
                ),
                Restrict.GetTypeRestriction(
                    this.Expression,
                    this.LimitType
                )
            );
        }

        public override Meta BindSetMember(SetMemberBinder binder, Meta value)
        {
            //TODO: insert defer

            return new Meta(
                Et.Call(
                    EtUtils.Cast<Obj>(this.Expression),
                    typeof(Obj).GetMethod("Put"),
                    Et.Constant(binder.Name),
                    EtUtils.Box(value.Expression),
                    Et.Constant(
                        (binder is JsSetMemberBinder)
                        ? ((JsSetMemberBinder)binder).Attrs
                        : 0
                    )
                ),
                Restrict.GetTypeRestriction(
                    this.Expression, 
                    this.LimitType
                )
            );
        }

        public override Meta BindInvokeMember(InvokeMemberBinder binder, Meta[] args)
        {
            //TODO: insert defer

            return new Meta(
                Et.Dynamic(
                    new JsInvokeBinder(
                        new CallInfo(args.Length + 2),
                        InvokeFlag.Method
                    ),
                    typeof(object),
                    ArrayUtils.Insert(
                        // The Js.Obj we shall call
                        Et.Dynamic(
                            new JsGetMemberBinder(binder.Name),
                            typeof(object),
                            this.Expression
                        ),
                        this.Expression, // 'this'-argument
                        DynamicUtils.GetExpressions(args) // other arguments
                    )
                ),
                RestrictUtils.BuildCallRestrictions(
                    this,
                    args,
                    RestrictFlag.Type
                )
            );
        }

        public override Meta BindInvoke(InvokeBinder binder, Meta[] args)
        {
            //TODO: insert defer

            if (binder is JsInvokeBinder)
            {
                var jsBinder = (JsInvokeBinder)binder;
                var selfExpr = EtUtils.Cast<Obj>(this.Expression);
                var selfObj = (Obj)this.Value;

                if (selfObj.Class == ObjClass.Object)
                {
                    // this expression handles
                    // the case where the object
                    // isn't a function
                    return EtUtils.CreateThrow(
                        this,
                        args,
                        Restrict.GetTypeRestriction(
                            this.Expression, 
                            typeof(Obj)
                        ).Merge(
                            Restrict.GetExpressionRestriction(
                                Et.Equal(
                                    Et.Field(
                                        selfExpr,
                                        "Class"
                                    ),
                                    Et.Constant(ObjClass.Object)
                                )
                            )
                        ),
                        typeof(RuntimeError),
                        "Object is not a function"
                    );
                }

                // this is the target delegate 
                // we're going to call
                var callTargetExpr = Et.Field(
                    EtUtils.Cast<Lambda>(
                        Et.Field(
                            selfExpr, 
                            "Lambda"
                        )
                    ), 
                    "Delegate"
                );

                // this is our new call frame
                var callFrameExpr = Et.Parameter(
                    typeof(Frame), 
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
                        Et.Field(
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

                Et exprTree;

                switch (jsBinder.CallType)
                {
                    case InvokeFlag.Function:
                        exprTree = SetupCallFrame(
                            exprs, 
                            callTargetExpr, 
                            callFrameExpr, 
                            argumentsExpr, 
                            selfObj.Lambda, 
                            args,
                            Et.Default(typeof(object))
                        );
                        break;

                    case InvokeFlag.Method:
                        exprTree = SetupCallFrame(
                            exprs,
                            callTargetExpr,
                            callFrameExpr,
                            argumentsExpr,
                            selfObj.Lambda,
                            ArrayUtils.RemoveFirst(args),
                            args[0].Expression
                        );
                        break;

                    case InvokeFlag.Constructor:
                        exprTree = SetupConstructorCallFrame(
                            exprs,
                            callTargetExpr,
                            callFrameExpr,
                            argumentsExpr,
                            selfObj.Lambda,
                            args
                        );
                        break;

                    default:
                        throw new NotImplementedException();
                }

                return new Meta(
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
                            Et.Field(
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

        private Et SetupConstructorCallFrame(List<Et> exprs, Et callTargetExpr, Parm callFrameExpr, Parm argumentsExpr, Lambda lambda, Meta[] args)
        {
            var thisParm = Et.Parameter(
                typeof(object), 
                "#this"
            );

            // create new object
            exprs.Add(
                Et.Assign(
                    thisParm,
                    Et.Call(
                        EtUtils.Cast<Obj>(this.Expression),
                        typeof(Obj).GetMethod("Construct")
                    )
                )
            );

            // hidden 'this' parameter
            exprs.Add(
                FrameUtils.Push(
                    callFrameExpr,
                    "this",
                    thisParm,
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

            // we want to return
            // the new object created
            // instead of the result
            // of the function call
            exprs.Add(thisParm);

            return Et.Block(
                new[] { callFrameExpr, argumentsExpr, thisParm },
                exprs
            );
        }

        private Et SetupCallFrame(List<Et> exprs, Et callTargetExpr, Parm callFrameExpr, Parm argumentsExpr, Lambda lambda, Meta[] args, Et that)
        {
            //TODO: extract methods from this + SetupFunctionCallFrame, duplicate functionality
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

        private void PushArgsOnFrame(List<Et> exprs, Parm callFrameExpr, Parm argumentsExpr, Lambda lambda, Meta[] args)
        {
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

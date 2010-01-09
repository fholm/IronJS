using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;
using System.Reflection;
using IronJS.Runtime.Binders;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Js
{
    using Et = System.Linq.Expressions.Expression;
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

        public override Meta BindInvoke(InvokeBinder binder, Meta[] args)
        {
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
                                    Et.Property(
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
                    Frame.Enter(
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
                        Obj.CreateNew()
                    )
                );

                switch (jsBinder.CallType)
                {
                    case InvokeFlag.Function:
                        SetupFunctionCallFrame(exprs, callFrameExpr, argumentsExpr, selfObj.Lambda, args);
                        break;

                    case InvokeFlag.Method:
                        SetupMethodCallFrame(exprs, callFrameExpr, argumentsExpr, selfObj.Lambda, args);
                        break;

                    case InvokeFlag.Constructor:
                        SetupConstructorCallFrame(exprs, callFrameExpr, argumentsExpr, selfObj.Lambda, args);
                        break;
                }

                // add the 'arguments' parameter
                // to the callframe
                exprs.Add(
                    Frame.Var(
                        callFrameExpr, 
                        "arguments", 
                        argumentsExpr, 
                        VarType.Local
                    )
                );

                // finally, emit the call
                // expression tree
                exprs.Add(
                    EtUtils.Box(
                        Et.Call(
                            callTargetExpr,
                            LambdaInvoke,
                            callFrameExpr
                        )
                    )
                );

                return new Meta(
                    Et.Block(
                        new[] { callFrameExpr, argumentsExpr },
                        exprs
                    ),

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

        private void SetupConstructorCallFrame(List<Et> exprs, ParameterExpression callFrameExpr, ParameterExpression argumentsExpr, Lambda lambda, Meta[] args)
        {
            throw new NotImplementedException();
        }

        private void SetupMethodCallFrame(List<Et> exprs, ParameterExpression callFrameExpr, ParameterExpression argumentsExpr, Lambda lambda, Meta[] args)
        {
            throw new NotImplementedException();
        }

        private void SetupFunctionCallFrame(List<Et> exprs, ParameterExpression callFrameExpr, ParameterExpression argumentsExpr, Lambda lambda, Meta[] args)
        {
            for (int i = 0; i < args.Length; ++i)
            {
                exprs.Add(
                    Frame.Var(
                        callFrameExpr,
                        lambda.Params[i],
                        args[i].Expression,
                        VarType.Local
                    )
                );

                exprs.Add(
                    Obj.SetMember(
                        argumentsExpr,
                        i,
                        args[i].Expression
                    )
                );
            }

            // 'arguments' parameter
            exprs.Add(
                Obj.SetMember(
                    argumentsExpr,
                    "callee",
                    this.Expression
                )
            );

            // hidden 'this' parameter
            exprs.Add(
                Frame.Var(
                    callFrameExpr,
                    "this",
                    Et.Default(typeof(object)),
                    VarType.Local
                )
            );
        }
    }
}

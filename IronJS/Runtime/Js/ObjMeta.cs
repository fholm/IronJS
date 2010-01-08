using System;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.Scripting.Utils;
using IronJS.Runtime.Utils;
using IronJS.Runtime.Binders;

namespace IronJS.Runtime.Js
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;
    using Restrict = System.Dynamic.BindingRestrictions;
    using AstUtils = Microsoft.Scripting.Ast.Utils;

    internal class ObjMeta : Meta
    {
        private Js.Obj JsObject;

        public ObjMeta(Et objExpr, Js.Obj jsObject)
            : base(objExpr, Restrict.Empty, jsObject)
        {
            JsObject = jsObject;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder"></param>
        /// <returns></returns>
        public override Meta BindGetMember(GetMemberBinder binder)
        {
            // call .Get on this expression
            var callExpr =
                Et.Call(
                    Et.Convert(this.Expression, typeof(Js.Obj)),
                    typeof(Js.Obj).GetMethod("Get"),
                    Et.Constant(binder.Name)
                );
            
            // This is valid for any 
            // object of this type
            var restrictions =
                Restrict.GetTypeRestriction(
                    this.Expression, 
                    this.LimitType
                );

            return new Meta(callExpr, restrictions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override Meta BindSetMember(SetMemberBinder binder, Meta value)
        {
            var keyExpr = Et.Constant(binder.Name);
            var valueExpr = EtUtils.Cast<object>(value.Expression);
            var attrsExpr = Et.Constant(
                                (binder is JsSetMemberBinder)
                                    ? ((JsSetMemberBinder)binder).Attrs
                                    : 0
                            );

            var target = Et.Call(
                EtUtils.Cast<Js.Obj>(this.Expression),
                typeof(Js.Obj).GetMethod("Put"),
                keyExpr, 
                valueExpr,
                attrsExpr
            );

            var restrictions = 
                Restrict.GetTypeRestriction(
                    this.Expression, 
                    this.LimitType
                );

            return new Meta(target, restrictions);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public override Meta BindInvokeMember(InvokeMemberBinder binder, Meta[] args)
        {
            var targetExprs =
                ArrayUtils.Insert(
                    // The Js.Obj we shall call
                    Et.Dynamic(
                        new JsGetMemberBinder(binder.Name),
                        typeof(object), 
                        this.Expression
                    ),
                    this.Expression, // 'this'-argument
                    DynamicUtils.GetExpressions(args) // other arguments
                );

            return new Meta(
                Et.Dynamic(
                    new JsInvokeBinder(
                        new CallInfo(targetExprs.Length), 
                        InvokeFlag.Method
                    ),
                    typeof(object),
                    targetExprs
                ),
                RestrictUtils.BuildCallRestrictions(
                    this,
                    args,
                    RestrictFlag.Type
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
            if (binder is Binders.JsInvokeBinder)
            {
                var invokeBinder = (Binders.JsInvokeBinder)binder;

                if (!JsObject.IsCallable)
                {
                    // this exception is valid
                    // if the objects is a Js.Obj and
                    // its IsCallable property
                    // returns false
                    var restriction = 
                        Restrict.GetTypeRestriction(
                            this.Expression, 
                            typeof(Js.Obj)
                        ).Merge(
                            Restrict.GetExpressionRestriction(
                                Et.Equal(
                                    Et.Property(
                                        EtUtils.Cast<Js.Obj>(this.Expression),
                                        "IsCallable"
                                    ),
                                    Et.Constant(false)
                                )
                            )
                        );

                    return EtUtils.CreateThrow(
                        this,
                        args,
                        restriction,
                        typeof(RuntimeError),
                        "Object isn't callable"
                    );
                }

                var callType = JsObject.Call.GetType();

                // this is the target delegate
                // for both normal and constructor calls
                var callTarget =
                    Et.PropertyOrField(
                        EtUtils.Cast<Js.Obj>(this.Expression),
                        "Call"
                    );

                // Restricts the callsite to
                // Js.Obj instances that have a Call
                // property of this exact type
                var callRestrict =
                    Restrict.GetTypeRestriction(
                        Expression.Field(
                            EtUtils.Cast<Js.Obj>(this.Expression),
                            "Call"
                        ),
                        callType
                    ).Merge(
                        RestrictUtils.BuildCallRestrictions(
                            this,
                            args,
                            RestrictFlag.Type
                        )
                    );

                var argExprs = DynamicUtils.GetExpressions(args);

                // handles constructor calls: "new foo();"
                if (invokeBinder.CallType == InvokeFlag.Constructor)
                {
                    var constructCall = AstUtils.SimpleCallHelper(
                        EtUtils.Cast<Js.Obj>(this.Expression),
                        typeof(Js.Obj).GetMethod("Construct")
                    );

                    // temp variable to store the
                    // new object reference
                    var tmp = 
                        Et.Variable(
                            typeof(object), 
                            "__tmp.NewObj__"
                        );

                    // call arguments with
                    // the first argument (this)
                    // set to the new object
                    var callArgs = 
                        ArrayUtils.Insert(
                            JsUtils.CreateJsArgsObj(
                                this.Expression, 
                                argExprs, 
                                JsObject.Context
                            ),
                            EtUtils.Cast<Js.Obj>(tmp),
                            argExprs
                        );

                    return new Meta(
                        Et.Block(
                            new[] { tmp },
                            Et.Assign(tmp, constructCall), // call Js.Obj.Construct and assign result to tmp
                            AstUtils.SimpleCallHelper( // this calls Js.Obj.Call with tmp as this parameter
                                callTarget,
                                callType.GetMethod("Invoke"),
                                callArgs
                            ),
                            tmp // return result
                        ),
                        callRestrict
                    );

                }

                // handles methods calls: "foo.bar();"
                else if(invokeBinder.CallType == InvokeFlag.Method)
                {
                    return new Meta(
                        AstUtils.SimpleCallHelper(
                            callTarget, 
                            callType.GetMethod("Invoke"), 

                            ArrayUtils.Insert(
                                JsUtils.CreateJsArgsObj(
                                    this.Expression,
                                    ArrayUtils.RemoveFirst(argExprs), // need to remove 'this' paremeter from 'arguments'-parameter
                                    JsObject.Context
                                ),
                                argExprs
                           )
                        ),
                        callRestrict
                    );
                }

                // handles function calls: "foo();"
                else if (invokeBinder.CallType == InvokeFlag.Function)
                {
                    return new Meta(
                        AstUtils.SimpleCallHelper(
                            callTarget,
                            callType.GetMethod("Invoke"),
                            ArrayUtils.Insert(
                                JsUtils.CreateJsArgsObj(
                                    this.Expression, 
                                    argExprs,
                                    JsObject.Context
                                ),
                                Et.Constant(JsObject.Context.Globals),
                                argExprs
                            )
                        ),
                        callRestrict
                    );
                }
            }

            return binder.FallbackInvoke(this, args);
        }

    }
}

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

    class IObjMeta : Meta
    {
        Context _context;

        //TODO: check all Get/Set operations it's ok to just type-restrict on the target

        public IObjMeta(Et parameter, IObj jsObj)
            : base(parameter, Restrict.Empty, jsObj)
        {

        }

        public override Meta BindSetIndex(SetIndexBinder binder, Meta[] indexes, Meta value)
        {
            //TODO: insert defer

            return new Meta(
                Et.Call(
                    EtUtils.Cast<Obj>(this.Expression),
                    typeof(Obj).GetMethod("Put"),
                    indexes[0].Expression,
                    EtUtils.Box(value.Expression)
                ),
                Restrict.GetTypeRestriction(
                    this.Expression,
                    this.LimitType
                )
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="indexes"></param>
        /// <returns></returns>
        public override Meta BindGetIndex(GetIndexBinder binder, Meta[] indexes)
        {
            //TODO: insert defer

            return new Meta(
                Et.Call(
                    EtUtils.Cast<Obj>(this.Expression),
                    typeof(Obj).GetMethod("Get"),
                    indexes[0].Expression
                ),
                Restrict.GetTypeRestriction(
                    this.Expression,
                    this.LimitType
                )
            );
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public override Meta BindSetMember(SetMemberBinder binder, Meta value)
        {
            //TODO: insert defer

            return new Meta(
                Et.Call(
                    EtUtils.Cast<Obj>(this.Expression),
                    typeof(Obj).GetMethod("PutWithAttrs"),
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="binder"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        public override Meta BindInvokeMember(InvokeMemberBinder binder, Meta[] args)
        {
            //TODO: insert defer

            return new Meta(
                Et.Dynamic(
                    _context.CreateInvokeBinder(
                        new CallInfo(args.Length + 2),
                        InvokeFlag.Method
                    ),
                    typeof(object),
                    ArrayUtils.Insert(
                        // the Js.Obj we're calling
                        Et.Call(
                            EtUtils.Cast<Obj>(this.Expression),
                            typeof(Obj).GetMethod("Get"),
                            Et.Constant(binder.Name)
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

     
        /*
        /// <summary>
        /// 
        /// </summary>
        /// <param name="exprs"></param>
        /// <param name="callTargetExpr"></param>
        /// <param name="callFrameExpr"></param>
        /// <param name="argumentsExpr"></param>
        /// <param name="lambda"></param>
        /// <param name="args"></param>
        /// <returns></returns>
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

            // push arguments on call new callframe
            PushArgsOnFrame(exprs, callFrameExpr, argumentsExpr, lambda, args);

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
        */
    }
}

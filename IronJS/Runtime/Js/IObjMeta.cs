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
            _context = jsObj.Context;
        }

        public override Meta BindSetIndex(SetIndexBinder binder, Meta[] indexes, Meta value)
        {
            //TODO: insert defer

            return new Meta(
                Et.Call(
                    EtUtils.Cast<IObj>(this.Expression),
                    typeof(IObj).GetMethod("Put"),
                    EtUtils.Box(indexes[0].Expression),
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
                    EtUtils.Cast<IObj>(this.Expression),
                    typeof(IObj).GetMethod("Get"),
                    EtUtils.Box(indexes[0].Expression)
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
                    EtUtils.Cast<IObj>(this.Expression),
                    typeof(IObj).GetMethod("Get"),
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
                    EtUtils.Cast<IObj>(this.Expression),
                    typeof(IObj).GetMethod("Put"),
                    Et.Constant(binder.Name),
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
        /// <param name="args"></param>
        /// <returns></returns>
        public override Meta BindInvokeMember(InvokeMemberBinder binder, Meta[] args)
        {
            //TODO: insert defer

            var selfExpr = EtUtils.Cast<IObj>(this.Expression);
            var tmp = Et.Parameter(typeof(object), "#tmp");

            return new Meta(
                Et.Block(
                    new[] { tmp },
                    Et.Assign(
                        tmp,
                        Et.Call(
                            selfExpr,
                            typeof(IObj).GetMethod("Get"),
                            Et.Constant(binder.Name)
                        )
                    ),
                    Et.Dynamic(
                        _context.CreateInvokeBinder(new CallInfo(args.Length)),
                        typeof(object),
                        ArrayUtils.Insert(
                            tmp,
                            selfExpr, 
                            DynamicUtils.GetExpressions(args)
                        )
                    )
                ),
                RestrictUtils.BuildCallRestrictions(
                    this,
                    args,
                    RestrictFlag.Type
                )
            );
        }

    }
}

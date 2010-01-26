using System.Dynamic;
using IronJS.Runtime.Utils;
using Microsoft.Scripting.Utils;
using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;
using Restrict = System.Dynamic.BindingRestrictions;

namespace IronJS.Runtime.Js
{
    //TODO: check all Get/Set operations it's ok to just type-restrict on the target

    class IObjMeta : Meta
    {
        Context _context;

        public IObjMeta(Et parameter, IObj jsObj)
            : base(parameter, Restrict.Empty, jsObj)
        {
            _context = jsObj.Context;
        }

        public override Meta BindSetIndex(SetIndexBinder binder, Meta[] indexes, Meta value)
        {
            return new Meta(
                Et.Call(
                    IObjUtils.MiSetObj,
                    EtUtils.Cast<IObj>(this.Expression),
                    Et.Call(
                        JsTypeConverter.MiToArrayIndex,
                        EtUtils.Box(indexes[0].Expression)
                    ),
                    EtUtils.Box(value.Expression)
                ),
                Restrict.GetTypeRestriction(
                    this.Expression,
                    this.LimitType
                )
            );
        }

        public override Meta BindGetIndex(GetIndexBinder binder, Meta[] indexes)
        {
            return new Meta(
                Et.Call(
                    IObjUtils.MiSearchObject,
                    EtUtils.Cast<IObj>(this.Expression),
                    Et.Call(
                        JsTypeConverter.MiToArrayIndex,
                        EtUtils.Box(indexes[0].Expression)
                    )
                ),
                Restrict.GetTypeRestriction(
                    this.Expression,
                    this.LimitType
                )
            );
        }

        public override Meta BindDeleteIndex(DeleteIndexBinder binder, Meta[] indexes)
        {
            return new Meta(
                Et.Call(
                    EtUtils.Cast<IObj>(this.Expression),
                    IObjUtils.MiTryDelete,
                    Et.Call(
                        JsTypeConverter.MiToArrayIndex,
                        EtUtils.Box(indexes[0].Expression)
                    )
                ),
                Restrict.GetTypeRestriction(
                    this.Expression,
                    this.LimitType
                )
            );
        }

        public override Meta BindGetMember(GetMemberBinder binder)
        {
            return new Meta(
                Et.Call(
                    IObjUtils.MiGet,
                    EtUtils.Cast<IObj>(this.Expression),
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
            return new Meta(
                Et.Call(
                    IObjUtils.MiSetObj,
                    EtUtils.Cast<IObj>(this.Expression),
                    Et.Constant(binder.Name),
                    EtUtils.Box(value.Expression)
                ),
                Restrict.GetTypeRestriction(
                    this.Expression, 
                    this.LimitType
                )
            );
        }

        public override Meta BindDeleteMember(DeleteMemberBinder binder)
        {
            return new Meta(
                Et.Call(
                    EtUtils.Cast<IObj>(this.Expression),
                    IObjUtils.MiTryDelete,
                    Et.Constant(binder.Name)
                ),
                Restrict.GetTypeRestriction(
                    this.Expression,
                    this.LimitType
                )
            );
        }

        public override Meta BindInvokeMember(InvokeMemberBinder binder, Meta[] args)
        {
            var selfExpr = EtUtils.Cast<IObj>(this.Expression);
            var tmp = Et.Parameter(typeof(object), "#tmp");

            return new Meta(
                Et.Block(
                    new[] { tmp },
                    Et.Assign(
                        tmp,
                        Et.Call(
                            IObjUtils.MiSearchObject,
                            selfExpr,
                            Et.Constant(binder.Name)
                        )
                    ),
                    Et.Dynamic(
                        _context.CreateInvokeBinder(new CallInfo(args.Length)),
                        typeof(object),
                        ArrayUtils.Insert(
                            tmp,
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

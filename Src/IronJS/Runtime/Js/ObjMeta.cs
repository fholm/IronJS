using System;
using System.Dynamic;
using IronJS.Tools;
using Microsoft.Scripting.Utils;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Runtime.Js {
	using Et = Expression;
	using MetaObj = DynamicMetaObject;

	public class ObjMeta : MetaObj {
		Obj _obj;

		public ObjMeta(Et expr, Obj obj) 
			: base(expr, BindingRestrictions.Empty, obj) {
			_obj = obj;
		}

        public override MetaObj BindInvoke(InvokeBinder binder, MetaObj[] args) {
            if (!_obj.IsCallable) {
                return new MetaObj(
                    Et.Throw(
                        AstTools.New(
                            typeof(RuntimeError),
                            AstTools.Constant("Object is not callable")
                        )
                    ),

                    BindingRestrictions.GetTypeRestriction(
                        Expression, typeof(Obj)
                    ).Merge(
                        BindingRestrictions.GetExpressionRestriction(
                            AstTools.FieldEq(Expression, "IsCallable", false)
                        )
                    )
                );
            }

            Type funcType = DelegateTools.BuildFuncType(
                ArrayUtils.Insert(_obj.Call.ClosureType, MetaObjTools.GetTypes(args))
            );

            Delegate compiled;
            if (!_obj.Call.Ast.JitCache.TryGet(funcType, _obj.Call, out compiled)) {
                compiled = _obj.Call.CompileAs(funcType);
            }

            return new MetaObj(
                Et.Invoke(
                    Et.Constant(compiled, funcType),
                    ArrayUtils.Insert(
                        Et.Constant(_obj.Call.Context, _obj.Call.ClosureType),
                        DynamicUtils.GetExpressions(args)
                    )
                ),
                BindingRestrictions.GetInstanceRestriction(Expression, _obj)
            );
        }
	}
}

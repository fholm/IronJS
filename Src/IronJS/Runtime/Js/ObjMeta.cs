using System;
using System.Dynamic;
using IronJS.Tools;
using IronJS.Runtime.Jit.Tools;
using Microsoft.Scripting.Utils;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Runtime.Js {
	using Et = Expression;
	using MetaObj = DynamicMetaObject;
    using Restrict = BindingRestrictions;

	public class ObjMeta : MetaObj {
		Obj _obj;

		public ObjMeta(Et expr, Obj obj) 
			: base(expr, BindingRestrictions.Empty, obj) {
			_obj = obj;
		}

        public override MetaObj BindInvoke(InvokeBinder binder, MetaObj[] args) {
            // This handles the rule for 
            // objects that are not callable
            if (!_obj.IsCallable) {
                return new MetaObj(
                    AstTools.Box(
                        Et.Throw(
                            AstTools.New(
                                typeof(RuntimeError),
                                AstTools.Constant("Object is not callable")
                            )
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

            // Get compiled deleegate from Closure
            Delegate compiled = _obj.Call.GetDelegate(args);

            // DLR Expressions
            // compiled.Invoke(closure, this, <args>)
            Et expression = Et.Invoke(
                Et.Constant(compiled, compiled.GetType()),
                ArrayUtils.Insert(
                    Et.Field(
                        Et.Field(
                            AstTools.Cast<Obj>(Expression), "Call"
                        ), 
                        "Context"
                    ),
                    DynamicUtils.GetExpressions(args)
                )
            );

            // Restrictions
            Restrict restrictions = Restrict.GetInstanceRestriction(
                Et.Field(
                    Et.Field(
                        AstTools.Cast<Obj>(Expression), "Call"
                    ),
                    "Ast"
                ),
                _obj.Call.Ast
            ).Merge(
                Restrict.GetTypeRestriction(
                    Et.Field(
                        Et.Field(
                            AstTools.Cast<Obj>(Expression), "Call"
                        ),
                        "Context"
                    ),
                    _obj.Call.ContextType
                )
            ).Merge(
                RestrictUtils.GetTypeRestrictions(args)
            );

            // 
            return new MetaObj(expression, restrictions);
        }
	}
}

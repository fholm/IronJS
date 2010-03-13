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

            // Build function type
            Type funcType = LambdaTools.BuildDelegateType(
                _obj.Call.Ast, 
                ArrayUtils.Insert(
                    _obj.Call.ContextType, MetaObjTools.GetTypes(args)
                )
            );

            // Get compiled delegate , from cache if  
            // possible or jit-compile it as funcType
            Delegate compiled;
            if (!_obj.Call.Ast.JitCache.TryGet(funcType, _obj.Call, out compiled)) {
                compiled = _obj.Call.CompileAs(funcType);
            }

            // DLR Expressions
            // compiled.Invoke(closure, this, <args>)
            Et expression = Et.Invoke(
                Et.Constant(compiled, funcType),
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

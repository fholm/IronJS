using System;
using System.Dynamic;
using IronJS.Tools;
using Microsoft.Scripting.Utils;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Runtime.Js.Meta {
	using Et = Expression;
	using MetaObj = DynamicMetaObject;

	public class FunctionMeta : ObjMeta {
		Function _func;

		public FunctionMeta(Et expr, Function func)
			: base(expr, func) {
			_func = func;
		}

		public override MetaObj BindInvoke(InvokeBinder binder, MetaObj[] args) {
			Type funcType = DelegateTools.BuildFuncType(
				ArrayUtils.Insert(_func.ClosureType, MetaObjTools.GetTypes(args))
			);

			Delegate compiled; 
			if (!_func.Cache.TryGetValue(funcType, out compiled)) {
				compiled = _func.CompileAs(funcType);
			}

			return new MetaObj(
				Et.Invoke(
					Et.Constant(compiled, funcType),
					ArrayUtils.Insert(
						Et.Constant(_func.Closure, _func.ClosureType),
						DynamicUtils.GetExpressions(args)
					)
				),
				BindingRestrictions.GetInstanceRestriction(Expression, _func)
			);
		}
	}
}

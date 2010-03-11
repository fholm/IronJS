using System;
using System.Dynamic;
using IronJS.Ast.Tools;
using IronJS.Runtime.Js;
using IronJS.Tools;
using Microsoft.Scripting.Utils;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Runtime.Binders {
	using Et = Expression;
	using MetaObj = DynamicMetaObject;

	public class InvokeBinder : System.Dynamic.InvokeBinder {
		public InvokeBinder(CallInfo callInfo)
			: base(callInfo) {

		}

		public override MetaObj FallbackInvoke(MetaObj target, MetaObj[] args, MetaObj error) {

			if (target.LimitType == typeof(Func<object, object>)) {
				return new MetaObj(
					Et.Call(
						Et.Convert(target.Expression, target.LimitType),
						typeof(Func<object, object>).GetMethod("Invoke"),
						AstTools.Box(args[1].Expression)
					),
					BindingRestrictions.GetInstanceRestriction(
						target.Expression,
						target.Value
					)
				);
			}

			throw new NotImplementedException();
		}
	}
}

using System;
using System.Dynamic;
using IronJS.Ast.Tools;
using IronJS.Runtime.Js;
using IronJS.Tools;

#if CLR2
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;
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
						AstTools.Box(args[0].Expression)
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

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
			throw new NotImplementedException();
		}
	}
}

using System.Dynamic;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Runtime.Js.Meta {
	using Et = Expression;
	using MetaObj = DynamicMetaObject;

	public class Meta<T> : MetaObj where T : class {
		protected T Self { get { return (T)Value; } }
		protected Et SelfExpr { get { return Et.Convert(Expression, typeof(T)); } }

		public Meta(Et expr, T value)
			: base(expr, BindingRestrictions.Empty, value) {

		}
	}
}

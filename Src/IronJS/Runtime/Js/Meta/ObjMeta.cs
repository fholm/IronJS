using System.Dynamic;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Runtime.Js.Meta {
	using Et = Expression;
	using MetaObj = DynamicMetaObject;

	public class ObjMeta : MetaObj {
		Obj _obj;

		public ObjMeta(Et expr, Obj obj) 
			: base(expr, BindingRestrictions.Empty, obj) {
			_obj = obj;
		}
	}
}

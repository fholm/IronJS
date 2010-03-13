using System.Collections.Generic;
using System.Dynamic;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Runtime.Js {
	using Et = Expression;
	using MetaObj = DynamicMetaObject;

	public class Obj : IDynamicMetaObjectProvider {
		public Dictionary<object, object> Properties =
			new Dictionary<object, object>();

        public Closure Call { get; private set; }
        public bool IsCallable { get { return Call != null; } }

        public Obj() {

        }

        public Obj(Closure call) {
            Call = call;
        }

		public void Set(object name, object value) {
			Properties[name] = value;
		}

		public object Get(object name) {
			object value;

			if (Properties.TryGetValue(name, out value))
				return value;

			return Undefined.Instance;
		}

		#region IDynamicMetaObjectProvider Members

		public virtual MetaObj GetMetaObject(Et parameter) {
			return new ObjMeta(parameter, this);
		}

		#endregion
	}
}

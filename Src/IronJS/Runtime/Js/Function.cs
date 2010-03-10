using System;
using System.Collections.Generic;
using IronJS.Ast.Nodes;
using IronJS.Tools;
using Microsoft.Scripting.Utils;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Runtime.Js {
	public class Function : Obj {
		public Lambda Ast { get; protected set; }
		public Closure Closure { get; protected set; }
		public Type ClosureType { get { return Closure.GetType(); } }

		public Func<Closure, object> Func0;
		public Dictionary<Type, Tuple<Delegate, Delegate>> FuncCache;

		public Function(Lambda node, Closure closure) {
			Ast = node;
			Closure = closure;
			FuncCache = new Dictionary<Type, Tuple<Delegate, Delegate>>();
		}

		public TFunc Compile<TFunc, TGuard>(object[] values, out TGuard guard)
			where TFunc : class
			where TGuard : class {

			Type[] types = typeof(TFunc).GetGenericArguments();
			Type[] paramTypes = ArrayTools.DropFirstAndLast(types);



			guard = null;
			return null;
		}
	}
}

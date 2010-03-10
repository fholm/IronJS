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

namespace IronJS.Runtime2.Js {
	public class IjsFunc : IjsObj {
		public Lambda Ast { get; protected set; }
		public IjsClosure Closure { get; protected set; }
		public Type ClosureType { get { return Closure.GetType(); } }

		public Func<IjsClosure, object> Func0;
		public Dictionary<Type, Tuple<Delegate, Delegate>> FuncCache;

		public IjsFunc(Lambda node, IjsClosure closure) {
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

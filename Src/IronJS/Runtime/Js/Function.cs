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
		public Lambda Lambda { get; protected set; }
		public Closure Closure { get; protected set; }
		public Type ClosureType { get { return Closure.GetType(); } }
		public Dictionary<Type, Tuple<Delegate, Delegate>> FuncCache;

		public Function(Lambda node, Closure closure) {
			Lambda = node;
			Closure = closure;
			FuncCache = new Dictionary<Type, Tuple<Delegate, Delegate>>();
		}
	}
}

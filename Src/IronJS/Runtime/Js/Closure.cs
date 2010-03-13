using System;
using System.Collections.Generic;
using System.Dynamic;
using IronJS.Ast.Nodes;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Runtime.Js {
	public class Closure {
        public readonly Lambda Ast;
        public readonly ClosureCtx Context;
        public Type ClosureType { get { return Context.GetType(); } }

		public Closure(Lambda ast, ClosureCtx closure) {
			Ast = ast;
			Context = closure;
		}

		internal Delegate CompileAs(Type funcType) {
            return Ast.JitCache.Save(funcType, ClosureType, Context.Context.Jit.Compile(funcType, Ast));
		}
	}
}

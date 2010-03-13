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
        public readonly Type ContextType;

		public Closure(Lambda ast, ClosureCtx ctx) {
			Ast = ast;
			Context = ctx;
            ContextType = ctx.GetType();
		}

		internal Delegate CompileAs(Type funcType) {
            return Ast.JitCache.Save(funcType, ContextType, Context.Runtime.Jit.Compile(funcType, Ast));
		}
	}
}

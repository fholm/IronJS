using System;
using System.Collections.Generic;
using System.Dynamic;
using IronJS.Ast.Nodes;
using IronJS.Runtime.Js.Meta;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Runtime.Js {
	using MetaObj = DynamicMetaObject;

	public class Function : Obj {
		public Lambda Ast { get; protected set; }
		public Closure Closure { get; protected set; }
		public Type ClosureType { get { return Closure.GetType(); } }
		public Dictionary<Type, Delegate> Cache { get; protected set; }

		public Function(Lambda ast, Closure closure) {
			Ast = ast;
			Closure = closure;
			Cache = new Dictionary<Type, Delegate>();
		}

		public override MetaObj GetMetaObject(Expression expr) {
			return new FunctionMeta(expr, this);
		}

		internal Delegate CompileAs(Type funcType) {
			return Cache[funcType] = (Delegate) Closure.Context.Compiler.Compile(funcType, Ast);
		}
	}
}

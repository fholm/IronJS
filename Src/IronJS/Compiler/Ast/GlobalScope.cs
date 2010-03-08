using System.Collections.Generic;
using IronJS.Runtime2.Js;
using Microsoft.Scripting.Utils;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast {
	public sealed class GlobalScope : Function {
		private GlobalScope(INode body)
			: base(null, new List<string>(), body, null) {

		}

		public Func<IjsClosure, object> Compile(IjsContext context) {
			Func<bool> guard;

			IjsFunc tempFunc = new IjsFunc(this, context.GlobalClosure);

			Func<IjsClosure, object> compiled =
				tempFunc.Compile<Func<IjsClosure, object>, Func<bool>>(
					ArrayUtils.EmptyObjects, out guard
				);

			return compiled;
		}

		public GlobalScope Analyze() {
			return (GlobalScope)Analyze(new Stack<Function>());
		}

		public static GlobalScope Create(List<INode> body) {
			return new GlobalScope(new Block(body, null));
		}
	}
}

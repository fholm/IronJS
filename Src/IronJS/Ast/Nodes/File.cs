using System.Collections.Generic;
using IronJS.Runtime2.Js;
using Microsoft.Scripting.Utils;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Nodes {
	public sealed class File : Lambda {
		private File(INode body)
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

		public File Analyze() {
			return (File)Analyze(new Stack<Lambda>());
		}

		public static File Create(List<INode> body) {
			return new File(new Block(body, null));
		}
	}
}

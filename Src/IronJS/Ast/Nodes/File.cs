using System.Collections.Generic;
using IronJS.Runtime.Js;
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

		public Func<Closure, object> Compile(Context context) {
			Func<bool> guard;

			Function tempFunc = new Function(this, context.GlobalClosure);

			Func<Closure, object> compiled =
				tempFunc.Compile<Func<Closure, object>, Func<bool>>(
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

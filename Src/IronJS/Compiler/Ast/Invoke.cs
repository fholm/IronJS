using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Compiler.Tools;
using IronJS.Runtime2.Js;
using Microsoft.Scripting.Utils;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast {
	using Et = Expression;

	public class Invoke : Node {
		public INode Target { get { return Children[0]; } }

		public Invoke(INode target, List<INode> args, ITree node)
			: base(NodeType.Call, node) {
			Children = new INode[args.Count + 1];
			Children[0] = target;
			args.CopyTo(Children, 1);
		}

		public override INode Analyze(Stack<Function> stack) {
			base.Analyze(stack);
			AnalyzeTools.IfIdentiferUsedAs(Target, IjsTypes.Object);
			return this;
		}

		public override Et Compile(Function func) {
			var args = ArrayUtils.RemoveFirst(Children);

			if (args.Length == 0)
				return CompileTools.Call0(func, Target);

			return CompileTools.CallN(func, Target, args);
		}
	}
}

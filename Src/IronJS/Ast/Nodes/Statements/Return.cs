using Antlr.Runtime.Tree;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Nodes {
    using Et = Expression;

	public class Return : Base {
		public INode Value { get { return Children[0]; } }

		public Return(INode value, ITree node)
			: base(NodeType.Return, node) {
			Children = new[] { value };
		}

        public override Expression Compile(Lambda func) {
            return Et.Return(
                func.ReturnLabel, Et.Convert(
                    Value.Compile(func), func.ReturnType
                )
            );
        }
	}
}

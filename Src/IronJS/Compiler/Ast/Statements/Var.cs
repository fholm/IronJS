using Antlr.Runtime.Tree;
using System.Collections.Generic;

namespace IronJS.Compiler.Ast {
    public class Var : Node {
        public readonly INode Target;

        public Var(INode target, ITree node)
            : base(NodeType.Var, node) {
            Target = target;
        }

        public override INode Analyze(Stack<Function> stack) {
            return base.Analyze(stack);
        }
    }
}

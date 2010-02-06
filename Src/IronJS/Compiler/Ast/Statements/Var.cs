using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Tools;

namespace IronJS.Compiler.Ast {
    public class Var : Node {
        public INode Target { get; protected set; }

        public Var(INode target, ITree node)
            : base(NodeType.Var, node) {
            Target = target;
        }

        public override INode Analyze(Stack<Function> stack) {
            Function function = stack.Peek();

            if (!function.IsGlobalScope) {
                Symbol symbol = Target as Symbol;

                if (symbol == null) {
                    Assign assign = Target as Assign;

                    if (assign == null) {
                        throw new AstCompilerError("Var must have Assign or Symbol child");
                    }

                    symbol = assign.Target as Symbol;
                }

                function[symbol.Name] = new Local(symbol.Name);
            }

            return Target.Analyze(stack);
        }

        public override void Write(System.Text.StringBuilder writer, int depth) {
            string indent = StringTools.Repeat(" ", depth * 2);

            writer.AppendLine(indent + "(Var");
            Target.Write(writer, depth + 1);
            writer.AppendLine(indent + ")");
        }
    }
}

using System.Collections.Generic;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class LambdaNode : Node
    {
        public List<IdentifierNode> Args { get; protected set; }
        public Node Body { get; protected set; }
        public string Name { get; protected set; }

        public LambdaNode(List<IdentifierNode> args, Node body, string name)
            : base(NodeType.Lambda)
        {
            Args = args;
            Body = body;
            Name = name;
        }

        public override Et Walk(EtGenerator etgen)
        {
            return null;
        }
    }
}

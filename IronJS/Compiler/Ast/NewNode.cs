using System.Collections.Generic;

namespace IronJS.Compiler.Ast
{
    using Et = System.Linq.Expressions.Expression;

    class NewNode : Node
    {
        public readonly Node Target;
        public readonly List<Node> Args;
        public readonly List<AutoPropertyNode> Properties;

        public bool HasProperties { get { return Properties != null; } }

        public NewNode(Node target, List<Node> args)
            : base(NodeType.New)
        {
            Target = target;
            Args = args;
        }

        public NewNode(IdentifierNode target)
            : this(target, new List<Node>())
        {

        }

        public NewNode(IdentifierNode targets, List<Node> args, List<AutoPropertyNode> properties)
            : this(targets, args)
        {
            Properties = properties;
        }

        public override Et Walk(EtGenerator etgen)
        {
            throw new System.NotImplementedException();
        }
    }
}

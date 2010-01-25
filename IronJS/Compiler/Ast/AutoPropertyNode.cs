using System;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class AutoPropertyNode : Node
    {
        public object Name { get; protected set; }
        public Node Value { get; protected set; }

        public AutoPropertyNode(object name, Node value)
            : base(NodeType.AutoProperty)
        {
            Name = name;
            Value = value;
        }

        public override Et Walk(EtGenerator etgen)
        {
            // This Walk()-method is never called
            // so no need to implement it
            throw new NotImplementedException();
        }
    }
}

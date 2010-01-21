using System;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    class AutoPropertyNode : Node
    {
        public readonly string Name;
        public readonly Node Value;

        public AutoPropertyNode(string name, Node value)
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

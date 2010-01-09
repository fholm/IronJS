using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Compiler.Ast
{
    using Et = System.Linq.Expressions.Expression;

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
            throw new NotImplementedException();
        }
    }
}

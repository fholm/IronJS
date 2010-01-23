using System;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    class RegexNode : Node
    {
        private string Regex; 

        public RegexNode(string regex)
            : base(NodeType.Regex)
        {
            Regex = regex;
        }

        public override Et Walk(EtGenerator etgen)
        {
            return Et.Constant(Regex);
        }
    }
}

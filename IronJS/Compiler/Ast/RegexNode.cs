using System;
using Et = System.Linq.Expressions.Expression;
using IronJS.Runtime;

namespace IronJS.Compiler.Ast
{
    class RegexNode : Node
    {
        private string Regex;
        private string Modifiers;

        public RegexNode(string regex)
            : base(NodeType.Regex)
        {
            var lastIndex = regex.LastIndexOf('/');
            Regex = regex.Substring(1, lastIndex - 1);
            Modifiers = regex.Substring(lastIndex + 1);
        }

        public override Et Walk(EtGenerator etgen)
        {
            return Et.Call(
                Et.Constant(etgen.Context),
                Context.MiCreateRegExp,
                Et.Constant(Regex, typeof(object)),
                Et.Constant(Modifiers, typeof(object))
            );
        }
    }
}

using System;
using System.Text;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class IdentifierNode : Node
    {
        public bool IsLocal { get; set; }
        public bool IsClosure { get; set; }
        public bool IsDefinition { get; set; }

        public string Name { get; protected set; }

        public IdentifierNode(string name)
            : base(NodeType.Identifier)
        {
            Name = name;
            IsLocal = false;
            IsDefinition = false;
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);
            writer.AppendLine(indentStr + "(" + Name + ")");
        }

        public override Et Walk(EtGenerator etgen)
        {
            return Et.Call(
                etgen.FunctionScope.ScopeExpr,
                Scope.MiPull,
                Et.Constant(Name, typeof(object))
            );
        }
    }
}

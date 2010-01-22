using System;
using System.Text;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{

    class IdentifierNode : Node
    {
        public readonly string Name;
        public bool IsLocal;

        public IdentifierNode(string name)
            : base(NodeType.Identifier)
        {
            Name = name;
            IsLocal = false;
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);
            writer.AppendLine(indentStr + "(" + Type + " " + Name + ")");
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

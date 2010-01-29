using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class IfNode : Node
    {
        public Node Test { get; protected set; }
        public Node TrueBranch { get; protected set; }
        public Node ElseBranch { get; protected set; }
        public bool HasElseBranch { get { return ElseBranch != null; } }
        public bool IsTernary { get; protected set; }

        public IfNode(Node test, Node trueBranch, Node elseBranch, bool isTernary, ITree node)
            : base(NodeType.If, node)
        {
            Test = test;
            TrueBranch = trueBranch;
            ElseBranch = elseBranch;
            IsTernary = isTernary;
        }

        public override Node Optimize(AstOptimizer astopt)
        {
            Test = Test.Optimize(astopt);
            TrueBranch = TrueBranch.Optimize(astopt);

            if(HasElseBranch)
                ElseBranch = ElseBranch.Optimize(astopt);

            return this;
        }

        public override Et Generate(EtGenerator etgen)
        {
            var trueBranch = TrueBranch.Generate(etgen);
            var elseBranch = etgen.WalkIfNotNull(ElseBranch);

            return Et.Condition(
                Et.Dynamic(
                    etgen.Context.CreateConvertBinder(typeof(bool)),
                    typeof(bool),
                    Test.Generate(etgen)
                ),
                EtUtils.Cast<object>(trueBranch),
                EtUtils.Cast<object>(elseBranch)
            );
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Type);

            Test.Print(writer, indent + 1);
            TrueBranch.Print(writer, indent + 1);

            if (ElseBranch != null)
                ElseBranch.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }

    }
}

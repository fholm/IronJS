using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class IfNode : Node
    {
        public INode Test { get; protected set; }
        public INode TrueBranch { get; protected set; }
        public INode ElseBranch { get; protected set; }
        public bool HasElseBranch { get { return ElseBranch != null; } }
        public bool IsTernary { get; protected set; }

        public IfNode(INode test, INode trueBranch, INode elseBranch, bool isTernary, ITree node)
            : base(NodeType.If, node)
        {
            Test = test;
            TrueBranch = trueBranch;
            ElseBranch = elseBranch;
            IsTernary = isTernary;
        }

        public override Type ExprType
        {
            get
            {
                if (IsTernary)
                {
                    if (TrueBranch.ExprType == ElseBranch.ExprType)
                        return TrueBranch.ExprType;
                }

                return IjsTypes.Dynamic;
            }
        }

        public override INode Analyze(IjsAstAnalyzer astopt)
        {
            Test = Test.Analyze(astopt);
            TrueBranch = TrueBranch.Analyze(astopt);

            if(HasElseBranch)
                ElseBranch = ElseBranch.Analyze(astopt);

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

            writer.AppendLine(indentStr + "(" + NodeType);

            Test.Print(writer, indent + 1);
            TrueBranch.Print(writer, indent + 1);

            if (ElseBranch != null)
                ElseBranch.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }

    }
}

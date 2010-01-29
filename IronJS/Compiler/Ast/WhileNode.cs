using System;
using System.Collections.Generic;
using System.Text;
using Antlr.Runtime.Tree;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public enum WhileType { DoWhile, While }

    public class WhileNode : LoopNode
    {
        public Node Test { get; protected set; }
        public Node Body { get; protected set; }
        public WhileType Loop { get; protected set; }

        public WhileNode(Node test, Node body, WhileType type, ITree node)
            : base(NodeType.While, node)
        {
            Test = test;
            Body = body;
            Loop = type;
        }

        public override Et LoopWalk(EtGenerator etgen)
        {
            Et loop = null;

            var test = Et.Dynamic(
                etgen.Context.CreateConvertBinder(typeof(bool)),
                typeof(bool),
                Test.Generate(etgen)
            );

            // while
            if (Loop == Ast.WhileType.While)
            {
                var body = Body.Generate(etgen);

                loop = AstUtils.While(
                    test,
                    body,
                    null,
                    etgen.FunctionScope.LabelScope.Break(),
                    etgen.FunctionScope.LabelScope.Continue()
                );
            }
            // do ... while
            else if (Loop == Ast.WhileType.DoWhile)
            {
                var bodyExprs = new List<Et>();

                bodyExprs.Add(Body.Generate(etgen));

                // test last, instead of first
                bodyExprs.Add(
                    Et.IfThenElse(
                        test,
                        Et.Continue(etgen.FunctionScope.LabelScope.Continue()),
                        Et.Break(etgen.FunctionScope.LabelScope.Break())
                    )
                );

                loop = Et.Loop(
                    Et.Block(bodyExprs),
                    etgen.FunctionScope.LabelScope.Break(),
                    etgen.FunctionScope.LabelScope.Continue()
                );
            }

            return loop;
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Loop);

            Test.Print(writer, indent + 1);
            Body.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}

using System;
using System.Text;
using System.Collections.Generic;
using Et = System.Linq.Expressions.Expression;
using AstUtils = Microsoft.Scripting.Ast.Utils;

namespace IronJS.Compiler.Ast
{
    enum WhileType { DoWhile, While }

    class WhileNode : LoopNode
    {
        public readonly Node Test;
        public readonly Node Body;
        public readonly WhileType Loop;

        public WhileNode(Node test, Node body, WhileType type)
            : base(NodeType.While)
        {
            Test = test;
            Body = body;
            Loop = type;
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Loop);

            Test.Print(writer, indent + 1);
            Body.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }

        public override Et LoopWalk(EtGenerator etgen)
        {
            Et loop = null;

            var test = Et.Dynamic(
                etgen.Context.CreateConvertBinder(typeof(bool)),
                typeof(bool),
                Test.Walk(etgen)
            );

            // while
            if (Loop == Ast.WhileType.While)
            {
                var body = Body.Walk(etgen);

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

                bodyExprs.Add(Body.Walk(etgen));

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
    }
}

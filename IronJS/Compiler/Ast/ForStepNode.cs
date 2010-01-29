using System;
using Et = System.Linq.Expressions.Expression;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using System.Linq.Expressions;

namespace IronJS.Compiler.Ast
{
    // 12.6.3
    public class ForStepNode : LoopNode
    {
        public Node Setup { get; protected set; }
        public Node Test { get; protected set; }
        public Node Incr { get; protected set; }
        public Node Body { get; protected set; }

        public ForStepNode(Node setup, Node test, Node incr, Node body)
            : base(NodeType.ForStep)
        {
            Setup = setup;
            Test = test;
            Incr = incr;
            Body = body;
        }

        public class Mutable<T>
        {
            public T Value;
        }

        public override Et LoopWalk(EtGenerator etgen)
        {
            /*
            var tmp = Et.Variable(typeof(int), "#tmp");

            return Et.Block(
                new[] { tmp },
                Et.Assign(
                    tmp,
                    Et.Constant(0, typeof(int))
                ),
                AstUtils.Loop(
                    Et.LessThan(
                        tmp,
                        Et.Constant(
                            10000000, 
                            typeof(int)
                        )
                    ),
                    Et.Assign(
                        tmp,
                        Et.Add(
                            tmp,
                            Et.Constant(1, typeof(int))
                        )
                    ),
                    AstUtils.Empty(),
                    AstUtils.Empty()
                )
            );
            */

            var field = typeof(Mutable<double>).GetField("Value");
            var tmp = Et.Variable(typeof(Mutable<double>), "#tmp");

            return Et.Block(
                new[] { tmp },
                Et.Assign(
                    tmp,
                    AstUtils.SimpleNewHelper(
                        typeof(Mutable<double>).GetConstructor(System.Type.EmptyTypes)
                    )
                ),
                Et.Assign(
                    Et.Field(
                        tmp,
                        field
                    ),
                    Et.Constant(0.0D, typeof(double))
                ),
                AstUtils.Loop(
                    Et.LessThan(
                        Et.Field(
                            tmp,
                            field
                        ),
                        Et.Constant(10000000.0D, typeof(double))
                    ),
                    Et.Assign(
                        Et.Field(
                            tmp,
                            field
                        ),
                        Et.Convert(
                            Et.Dynamic(
                                etgen.Context.CreateBinaryOpBinder(ExpressionType.Add),
                                typeof(object),
                                Et.Field(
                                    tmp,
                                    field
                                ),
                                Et.Constant(1.0D, typeof(object))
                            ),
                            typeof(double)
                        )
                    ),
                    AstUtils.Empty(),
                    AstUtils.Empty()
                )
            );

            return Et.Block(
                Setup.Walk(etgen),
                AstUtils.Loop(
                    Et.Dynamic(
                        etgen.Context.CreateConvertBinder(typeof(bool)),
                        typeof(bool),
                        Test.Walk(etgen)
                    ),
                    Incr.Walk(etgen),
                    Body.Walk(etgen),
                    null,
                    etgen.FunctionScope.LabelScope.Break(),
                    etgen.FunctionScope.LabelScope.Continue()
                )
            );
        }

        public override void Print(System.Text.StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Type);

            Setup.Print(writer, indent + 1);
            Test.Print(writer, indent + 1);
            Incr.Print(writer, indent + 1);
            Body.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}

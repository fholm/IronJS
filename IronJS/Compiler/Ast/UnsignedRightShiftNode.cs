using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class UnsignedRightShiftNode : Node
    {
        public Node Left { get; protected set; }
        public Node Right { get; protected set; }

        public UnsignedRightShiftNode(Node left, Node right, ITree node)
            : base(NodeType.UnsignedRightShift, node)
        {
            Left = left;
            Right = right;
        }

        public override Et Generate(EtGenerator etgen)
        {
            //TODO: to much boxing/conversion going on
            return EtUtils.Box(
                Et.Convert(
                    Et.Call(
                        typeof(Operators).GetMethod("UnsignedRightShift"),

                        Et.Convert(
                            Et.Dynamic(
                                etgen.Context.CreateConvertBinder(typeof(double)),
                                typeof(double),
                                Left.Generate(etgen)
                            ),
                            typeof(int)
                        ),

                        Et.Convert(
                            Et.Dynamic(
                                etgen.Context.CreateConvertBinder(typeof(double)),
                                typeof(double),
                                Right.Generate(etgen)
                            ),
                            typeof(int)
                        )

                    ),
                    typeof(double)
                )
            );
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Type);

            Left.Print(writer, indent + 1);
            Right.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}

using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;
using System.Text;
using System;

namespace IronJS.Compiler.Ast
{
    public class UnsignedRightShiftNode : Node
    {
        public Node Left { get; protected set; }
        public Node Right { get; protected set; }

        public UnsignedRightShiftNode(Node left, Node right)
            : base(NodeType.UnsignedRightShift)
        {
            Left = left;
            Right = right;
        }

        public override Et Walk(EtGenerator etgen)
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
                                Left.Walk(etgen)
                            ),
                            typeof(int)
                        ),

                        Et.Convert(
                            Et.Dynamic(
                                etgen.Context.CreateConvertBinder(typeof(double)),
                                typeof(double),
                                Right.Walk(etgen)
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

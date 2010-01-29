using IronJS.Runtime;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;
using System;
using System.Text;

namespace IronJS.Compiler.Ast
{
    public class TryNode : Node
    {
        public Node Body { get; protected set; }
        public CatchNode Catch { get; protected set; }
        public Node Finally { get; protected set; }

        public TryNode(Node body, CatchNode _catch, Node _finally)
            : base(NodeType.Try)
        {
            Body = body;
            Catch = _catch;
            Finally = _finally;
        }

        public override Et Walk(EtGenerator etgen)
        {
            // try ... finally
            if (Catch == null)
            {
                return Et.TryFinally(
                    Body.Walk(etgen),
                    Finally.Walk(etgen)
                );
            }
            else
            {
                var catchParam = Et.Parameter(typeof(JsRuntimeError), "#catch");

                var catchBody = Et.Block(
                    etgen.GenerateAssign(
                        Catch.Target,
                        Et.Property(
                            catchParam,
                            "JsObj"
                        )
                    ),
                    Et.Block(
                        Catch.Body.Walk(etgen)
                    )
                );

                var catchBlock = Et.Catch(
                    catchParam,
                    EtUtils.Cast<object>(catchBody)
                );

                var tryBody = EtUtils.Box(Body.Walk(etgen));

                // try ... catch 
                if (Finally == null)
                {
                    return Et.TryCatch(
                        tryBody,
                        catchBlock
                    );
                }
                // try ... catch ... finally
                else
                {
                    return Et.TryCatchFinally(
                        tryBody,
                        Finally.Walk(etgen),
                        catchBlock
                    );
                }
            }
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + Type);

            Body.Print(writer, indent + 1);

            if (Catch != null)
                Catch.Print(writer, indent + 1);

            if (Finally != null)
            {
                var indentStr2 = new String(' ', (indent + 1) * 2);

                writer.AppendLine(indentStr2 + "(Finally");
                    Finally.Print(writer, indent + 2);
                writer.AppendLine(indentStr2 + ")");
            }

            writer.AppendLine(indentStr + ")");
        }
    }
}

using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class TryNode : Node
    {
        public INode Body { get; protected set; }
        public CatchNode Catch { get; protected set; }
        public INode Finally { get; protected set; }

        public TryNode(INode body, CatchNode _catch, INode _finally, ITree node)
            : base(NodeType.Try, node)
        {
            Body = body;
            Catch = _catch;
            Finally = _finally;
        }

        public override Et Generate(EtGenerator etgen)
        {
            // try ... finally
            if (Catch == null)
            {
                return Et.TryFinally(
                    Body.Generate(etgen),
                    Finally.Generate(etgen)
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
                        Catch.Body.Generate(etgen)
                    )
                );

                var catchBlock = Et.Catch(
                    catchParam,
                    EtUtils.Cast<object>(catchBody)
                );

                var tryBody = EtUtils.Box(Body.Generate(etgen));

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
                        Finally.Generate(etgen),
                        catchBlock
                    );
                }
            }
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType);

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

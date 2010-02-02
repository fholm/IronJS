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
        public INode Target { get; protected set; } 
        public INode Catch { get; protected set; }
        public INode Finally { get; protected set; }

        public TryNode(INode body, INode target, INode _catch, INode _finally, ITree node)
            : base(NodeType.Try, node)
        {
            Body = body;
            Catch = _catch;
            Target = target;
            Finally = _finally;
        }

        public override INode Analyze(FuncNode astopt)
        {
            Body = Body.Analyze(astopt);

            if (Target != null)
                Target = Target.Analyze(astopt);

            if (Catch != null)
                Catch = Catch.Analyze(astopt);

            if (Finally != null)
                Finally = Finally.Analyze(astopt);

            IfIdentiferUsedAs(Target, IjsTypes.Object);

            return this;
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

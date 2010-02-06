#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast {

    using System;
    using System.Collections.Generic;
    using System.Text;
    using Antlr.Runtime.Tree;
    using IronJS.Runtime2.Js;

    public class Try : Node {
        public INode Body { get; protected set; }
        public INode Target { get; protected set; }
        public INode Catch { get; protected set; }
        public INode Finally { get; protected set; }

        public Try(INode body, INode target, INode _catch, INode _finally, ITree node)
            : base(NodeType.Try, node) {
            Body = body;
            Catch = _catch;
            Target = target;
            Finally = _finally;
        }

        public override INode Analyze(Stack<Function> astopt) {
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

        public override void Write(StringBuilder writer, int indent) {
            string indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType);

            Body.Write(writer, indent + 1);

            if (Catch != null)
                Catch.Write(writer, indent + 1);

            if (Finally != null) {
                string indentStr2 = new String(' ', (indent + 1) * 2);

                writer.AppendLine(indentStr2 + "(Finally");
                Finally.Write(writer, indent + 2);
                writer.AppendLine(indentStr2 + ")");
            }

            writer.AppendLine(indentStr + ")");
        }
    }
}

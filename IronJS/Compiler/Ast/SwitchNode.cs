using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class SwitchNode : Node, ILabelableNode
    {
        public INode Target { get; protected set; }
        public INode Default { get; protected set; }
        public List<Tuple<INode, INode>> Cases { get; protected set; }
        public string Label { get; protected set; }

        public SwitchNode(INode taret, INode _default, List<Tuple<INode, INode>> cases, ITree node)
            : base(NodeType.Switch, node)
        {
            Target = taret;
            Default = _default;
            Cases = cases;
            Label = null;
        }

        public override INode Analyze(IjsAstAnalyzer astopt)
        {
            Target = Target.Analyze(astopt);

            if(Default != null)
                Default = Default.Analyze(astopt);

            var cases = new List<Tuple<INode, INode>>();

            foreach (var _case in Cases)
            {
                cases.Add(
                    Tuple.Create(
                        _case.Item1.Analyze(astopt),
                        _case.Item2.Analyze(astopt)
                    )
                );
            }

            Cases = cases;

            return this;
        }

        public override void Print(System.Text.StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);
            var indentStr2 = new String(' ', (indent + 1)* 2);
            var indentStr3 = new String(' ', (indent + 2) * 2);

            writer.AppendLine(indentStr + "(" + NodeType + "");
            Target.Print(writer, indent + 1);

            foreach (var cas in Cases)
            {
                writer.AppendLine(indentStr2 + "(Case");
                cas.Item1.Print(writer, indent + 2);
                cas.Item2.Print(writer, indent + 2);
                writer.AppendLine(indentStr2 + ")");
            }

            if (Default != null)
            {
                writer.AppendLine(indentStr2 + "(Default");
                Default.Print(writer, indent + 2);
                writer.AppendLine(indentStr2 + ")");
            }

            writer.AppendLine(indentStr + ")");
        }

        #region ILabelableNode Members

        public void SetLabel(string label)
        {
            Label = null;
        }

        #endregion
    }
}

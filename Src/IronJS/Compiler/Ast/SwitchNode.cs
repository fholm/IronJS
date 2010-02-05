using System;
using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Tools;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

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

        public override INode Analyze(FuncNode astopt)
        {
            Target = Target.Analyze(astopt);

            if(Default != null)
                Default = Default.Analyze(astopt);

            List<Tuple<INode, INode>> cases = new List<Tuple<INode, INode>>();

            foreach (Tuple<INode, INode> _case in Cases)
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

        public override void Print(System.Text.StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);
            string indentStr2 = new String(' ', (indent + 1)* 2);
            string indentStr3 = new String(' ', (indent + 2) * 2);

            writer.AppendLine(indentStr + "(" + NodeType + "");
            Target.Print(writer, indent + 1);

            foreach (Tuple<INode, INode> cas in Cases)
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

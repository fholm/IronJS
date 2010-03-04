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
    public class Switch : Node, ILabelable
    {
        public INode Target { get; protected set; }
        public INode Default { get; protected set; }
        public List<Tuple<INode, INode>> Cases { get; protected set; }
        public string Label { get; protected set; }

        public Switch(INode taret, INode _default, List<Tuple<INode, INode>> cases, ITree node)
            : base(NodeType.Switch, node)
        {
            Target = taret;
            Default = _default;
            Cases = cases;
            Label = null;
        }

        public override INode Analyze(Stack<Function> astopt)
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

        #region ILabelableNode Members

        public void SetLabel(string label)
        {
            Label = null;
        }

        #endregion
    }
}

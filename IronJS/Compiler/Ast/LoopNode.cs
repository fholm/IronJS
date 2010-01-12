using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Compiler.Ast
{
    abstract class LoopNode : Node, ILabelableNode
    {
        public string Label { get; protected set; }

        public LoopNode(NodeType type)
            : base(type)
        {

        }

        #region ILabelableNode Members

        public void SetLabel(string label)
        {
            Label = label;
        }

        public void Init(FunctionScope functionScope)
        {
            functionScope.EnterLabelScope(Label, true);
        }

        #endregion
    }
}

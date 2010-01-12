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

        public bool IsLabeled
        {
            get { return true; }
        }

        public void SetLabel(string label)
        {
            Label = label;
        }

        public void Exit(FunctionScope functionScope)
        {
            functionScope.ExitLabelScope();
        }

        public void Enter(FunctionScope functionScope)
        {
            functionScope.EnterLabelScope(Label, true);
        }

        #endregion
    }
}

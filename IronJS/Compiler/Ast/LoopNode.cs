using Et = System.Linq.Expressions.Expression;

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
        
        #endregion

        public override Et Walk(EtGenerator etgen)
        {
            etgen.FunctionScope.EnterLabelScope(Label, true);
            var et = LoopWalk(etgen);
            etgen.FunctionScope.ExitLabelScope();
            return et;
        }

        abstract public Et LoopWalk(EtGenerator etgen);
    }
}

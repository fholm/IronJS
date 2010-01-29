using Antlr.Runtime.Tree;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    abstract public class LoopNode : Node, ILabelableNode
    {
        public string Label { get; protected set; }

        public LoopNode(NodeType type, ITree node)
            : base(type, node)
        {

        }

        public override Et Generate(EtGenerator etgen)
        {
            etgen.FunctionScope.EnterLabelScope(Label, true);
            var et = LoopWalk(etgen);
            etgen.FunctionScope.ExitLabelScope();
            return et;
        }

        #region ILabelableNode Members

        public void SetLabel(string label)
        {
            Label = label;
        }
        
        #endregion

        abstract public Et LoopWalk(EtGenerator etgen);
    }
}

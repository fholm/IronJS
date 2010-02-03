using Antlr.Runtime.Tree;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Et = System.Linq.Expressions.Expression;
using IronJS.Runtime2.Js;

namespace IronJS.Compiler.Ast
{
    abstract public class LoopNode : Node, ILabelableNode
    {
        public string Label { get; protected set; }

        public LoopNode(NodeType type, ITree node)
            : base(type, node)
        {

        }

        public virtual Et LoopWalk(FuncNode etgen)
        {
            return AstUtils.Empty();
        }

        #region ILabelableNode Members

        public void SetLabel(string label)
        {
            Label = label;
        }
        
        #endregion
    }
}

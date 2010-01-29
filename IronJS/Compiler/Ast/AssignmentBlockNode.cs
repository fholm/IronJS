using System.Collections.Generic;
using Antlr.Runtime.Tree;

namespace IronJS.Compiler.Ast
{
    public class AssignmentBlockNode : BlockNode
    {
        public bool IsLocal { get; protected set; }

        public AssignmentBlockNode(List<INode> nodes, bool isLocal, ITree node)
            : base(nodes, node)
        {
            IsLocal = isLocal;
        }
    }
}

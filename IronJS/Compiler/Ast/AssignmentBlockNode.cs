using System.Collections.Generic;

namespace IronJS.Compiler.Ast
{
    public class AssignmentBlockNode : BlockNode
    {
        public bool IsLocal { get; protected set; }

        public AssignmentBlockNode(List<Node> nodes, bool isLocal)
            : base(nodes)
        {
            IsLocal = isLocal;
        }
    }
}

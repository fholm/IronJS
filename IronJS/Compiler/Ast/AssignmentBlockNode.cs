using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;

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

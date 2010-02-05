using System;
using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Binders;
using IronJS.Runtime2.Js;
using IronJS.Tools;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = Expression;
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

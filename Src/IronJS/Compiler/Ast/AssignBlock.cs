﻿using System.Collections.Generic;
using Antlr.Runtime.Tree;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast {
    public class AssignBlock : Block {
        public bool IsLocal { get; protected set; }

        public AssignBlock(List<INode> nodes, bool isLocal, ITree node)
            : base(nodes, node) {
            IsLocal = isLocal;
        }
    }
}

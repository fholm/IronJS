﻿using System.Collections.Generic;
using Antlr.Runtime.Tree;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Nodes {
	public class VarBlock : Block {
		public bool IsLocal { get; protected set; }

		public VarBlock(List<INode> nodes, bool isLocal, ITree node)
			: base(nodes, node) {
			IsLocal = isLocal;
		}
	}
}

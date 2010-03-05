using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Tools;
using System.Collections.Generic;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast
{
    using Et = Expression;

    public class Return : Node
    {
		public INode Value { get { return Children[0]; } }
        public Function FuncNode { get; protected set; }

        public Return(INode value, ITree node)
            : base(NodeType.Return, node)
        {
			Children = new[] { value };
        }
    }
}

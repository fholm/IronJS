using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast
{
    public class With : Node
    {
		public INode Target { get { return Children[0]; } }
		public INode Body { get { return Children[1]; } }

        public With(INode target, INode body, ITree node)
            : base(NodeType.With, node)
        {
			Children = new[] { target, body };
        }
    }
}

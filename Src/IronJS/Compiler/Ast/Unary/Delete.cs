using System;
using System.Collections.Generic;
using System.Text;
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

    public class Delete : Node
    {
		public INode Target { get { return Children[0]; } }

        public Delete(INode target, ITree node)
            : base(NodeType.Delete, node)
        {
			Children = new[] { target };
        }

        public override Type Type
        {
            get
            {
                return IjsTypes.Boolean;
            }
        }
    }
}

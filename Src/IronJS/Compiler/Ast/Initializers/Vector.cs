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

    public class Vector : Node, INode
    {
        public List<INode> Values { get; protected set; }

        public Vector(List<INode> values, ITree node)
            : base(NodeType.Array, node)
        {
            Values = values;
        }

        public override Type Type
        {
            get
            {
                return IjsTypes.Object;
            }
        }

        public override INode Analyze(Stack<Function> astopt)
        {
            for (int valuePos = 0; valuePos < Values.Count; ++valuePos)
                Values[valuePos] = Values[valuePos].Analyze(astopt);

            return this;
        }
    }
}

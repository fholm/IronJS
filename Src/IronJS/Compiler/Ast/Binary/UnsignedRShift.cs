using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;

#if CLR2
using Microsoft.Scripting.Ast;
using System.Collections.Generic;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast
{
    public class UnsignedRShift : Node
    {
        public INode Left { get; protected set; }
        public INode Right { get; protected set; }

        public UnsignedRShift(INode left, INode right, ITree node)
            : base(NodeType.UnsignedRightShift, node)
        {
            Left = left;
            Right = right;
        }

        public override Type Type
        {
            get
            {
                return IjsTypes.Integer;
            }
        }

        public override INode Analyze(Stack<Function> astopt)
        {
            Left = Left.Analyze(astopt);
            Right = Right.Analyze(astopt);

            IfIdentifierAssignedFrom(Left, Right);
            IfIdentifierAssignedFrom(Right, Left);

            return this;
        }

        public override void Write(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType);

            Left.Write(writer, indent + 1);
            Right.Write(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}

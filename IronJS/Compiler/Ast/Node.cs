using System;
using System.Text;
using Antlr.Runtime.Tree;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public enum NodeType
    {
        Assign, Identifier, Double, Null,
        MemberAccess, Call, If, Eq, Block,
        String, Lambda, While, BinaryOp,
        Object, New, AutoProperty, Return,
        UnaryOp, Logical, PostfixOperator,
        TypeOf, Boolean, Void, StrictCompare,
        UnsignedRightShift, ForStep, ForIn,
        Break, Continue, With, Try, Catch,
        Throw, IndexAccess, Delete, In,
        Switch, InstanceOf, Regex, Array,
        Integer
    }

    abstract public class Node
    {
        private NodeType nodeType;
        private ITree node;

        public NodeType Type { get; protected set; }
        public int Line { get; protected set; }
        public int Column { get; protected set; }

        public Node(NodeType type, ITree node)
        {
            Type = type;

            if (node != null)
            {
                Line = node.Line;
                Column = node.CharPositionInLine;
            }
            else
            {
                Line = -1;
                Column = -1;
            }
        }

        public string Print()
        {
            var writer = new StringBuilder();

            Print(writer);

            return writer.ToString();
        }

        public virtual void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);
            writer.AppendLine(indentStr + "(" + Type + ")");
        }

        public virtual Node Optimize(AstOptimizer astopt)
        {
            return this;
        }

        public override string ToString()
        {
            return Type.ToString();
        }

        #region abstract

        public abstract Et Generate(EtGenerator etgen);

        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;
using IronJS.Tools;
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

    public enum NodeType
    {
        Assign, Identifier, Double, Null,
        MemberAccess, Call, If, Eq, Block,
        String, Func, While, BinaryOp,
        Object, New, AutoProperty, Return,
        UnaryOp, Logical, PostfixOperator,
        TypeOf, Boolean, Void, StrictCompare,
        UnsignedRightShift, ForStep, ForIn,
        Break, Continue, With, Try, Catch,
        Throw, IndexAccess, Delete, In,
        Switch, InstanceOf, Regex, Array,
        Integer
    }

    abstract public class Node : INode
    {
        public NodeType NodeType { get; protected set; }
        public int Line { get; protected set; }
        public int Column { get; protected set; }
        public virtual Type ExprType { get { return IjsTypes.Dynamic; } }

        public Node(NodeType type, ITree node)
        {
            NodeType = type;

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

        public bool IdenticalTypes(params INode[] nodes)
        {
            if (nodes.Length > 0)
            {
                Type type = nodes[0].ExprType;

                for (int index = 0; index < nodes.Length; ++index)
                    if (nodes[index].ExprType != type)
                        return false;

                return true;
            }

            return false;
        }
        
        public Type EvalTypes(params INode[] nodes)
        {
            HashSet<Type> set = new HashSet<Type>();

            foreach (INode node in nodes)
                set.Add(node.ExprType);

            return HashSetTools.EvalType(set);
        }

        public void IfIdentifierAssignedFrom(INode node, INode value)
        {
            IdentifierNode idNode = node as IdentifierNode;

            if (idNode != null)
            {
                IjsLocalVar varInfo = idNode.VarInfo as IjsLocalVar;

                if (varInfo != null)
                    varInfo.AssignedFrom.Add(value);
            }
        }

        public void IfIdentiferUsedAs(INode node, Type type)
        {
            IdentifierNode idNode = node as IdentifierNode;

            if (idNode != null)
            {
                IjsLocalVar varInfo = idNode.VarInfo as IjsLocalVar;

                if (varInfo != null)
                    varInfo.UsedAs.Add(type);
            }
        }

        public string Print()
        {
            StringBuilder writer = new StringBuilder();

            Print(writer, 0);

            return writer.ToString();
        }

        public virtual void Print(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);
            writer.AppendLine(indentStr + "(" + NodeType + ")");
        }

        public virtual INode Analyze(FuncNode func)
        {
            return this;
        }

        public virtual Et Compile(FuncNode func)
        {
            return AstUtils.Empty();
        }
    }
}

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
        UnsignedRShift, ForStep, ForIn,
        Break, Continue, With, Try, Catch,
        Throw, IndexAccess, Delete, In,
        Switch, InstanceOf, Regex, Array,
        Integer, Var, Parameter, Local,
        Global, Closed
    }

    abstract public class Node : INode
    {
        public NodeType NodeType { get; protected set; }
        public int Line { get; protected set; }
        public int Column { get; protected set; }
        public virtual Type Type { get { return IjsTypes.Dynamic; } }

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

        public virtual INode Analyze(Stack<Function> func)
        {
            return this;
        }

        public virtual Et Compile(Function func)
        {
            return AstUtils.Empty();
        }

        public string Print() {
            StringBuilder writer = new StringBuilder();

            Write(writer, 0);

            return writer.ToString();
        }

        public virtual void Write(StringBuilder writer, int depth) {
            writer.AppendLine(
                StringTools.Repeat(" ", depth * 2) +
                "(" +
                    TypeTools.ShortName(GetType()) +
                ")"
            );
        }

        protected bool IdenticalTypes(params INode[] nodes) {
            if (nodes.Length > 0) {
                Type type = nodes[0].Type;

                for (int index = 0; index < nodes.Length; ++index)
                    if (nodes[index].Type != type)
                        return false;

                return true;
            }

            return false;
        }

        protected Type EvalTypes(params INode[] nodes) {
            HashSet<Type> set = new HashSet<Type>();

            foreach (INode node in nodes)
                set.Add(node.Type);

            return HashSetTools.EvalType(set);
        }

        protected void IfIdentifierAssignedFrom(INode node, INode value) {
            Variable variable = node as Variable;

            if (variable != null) {
                variable.AssignedFrom(value);
            }
        }

        protected void IfIdentiferUsedAs(INode node, Type type) {
            Variable variable = node as Variable;

            if (variable != null) {
                variable.UsedAs(type);
            }
        }
    }
}

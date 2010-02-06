using System;
using System.Text;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast
{
    using Et = Expression;

    public interface INode
    {
        int Line { get; }
        int Column { get; }
        NodeType NodeType { get; }
        Type ExprType { get; }

        Et Compile(Function func);
        INode Analyze(Function func);
        Type EvalTypes(params INode[] nodes);
        bool IdenticalTypes(params INode[] nodes);

        string Print();
        void Print(StringBuilder writer, int indent);
    }
}

using System;
using System.Text;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public interface INode
    {
        int Line { get; }
        int Column { get; }
        NodeType NodeType { get; }
        Type ExprType { get; }

        Et EtGen(IjsEtGenerator etgen);
        Et Generate(EtGenerator etgen);
        INode Analyze(IjsAstAnalyzer astopt);
        Type EvalTypes(params INode[] nodes);
        bool IdenticalTypes(params INode[] nodes);

        string Print();
        void Print(StringBuilder writer, int indent = 0);
    }
}

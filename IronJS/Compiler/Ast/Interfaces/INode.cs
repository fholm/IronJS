using System.Text;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public enum JsType { String, Integer, Double, Boolean, Null, Undefined, Object, Dynamic, Self }

    public interface INode
    {
        int Line { get; }
        int Column { get; }
        NodeType NodeType { get; }
        JsType ExprType { get; }

        Et Generate(EtGenerator etgen);
        Et Generate2(EtGenerator etgen);
        INode Optimize(AstOptimizer astopt);
        JsType EvalTypes(params INode[] nodes);
        bool IdenticalTypes(params INode[] nodes);

        string Print();
        void Print(StringBuilder writer, int indent = 0);
    }
}

using System;
using System.Text;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public enum JsType { String, Integer, Double, Boolean, Null, Undefined, Object, Dynamic, Self }

    public class JsTypes
    {
        public static readonly Type String = typeof(string);
        public static readonly Type Integer = typeof(int);
        public static readonly Type Double = typeof(double);
        public static readonly Type Boolean = typeof(bool)
        public static readonly Type Null = typeof(object);
        public static readonly Type Undefined = typeof(Undefined);
        public static readonly Type Object = typeof(JsObj);
        public static readonly Type Dynamic = typeof(object);
    }

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

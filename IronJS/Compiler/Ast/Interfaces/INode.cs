using System;
using System.Linq;
using System.Text;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public static class SelfMarker
    {

    }

    public static class JsTypes
    {
        public static readonly Type String = typeof(string);
        public static readonly Type Integer = typeof(int);
        public static readonly Type Double = typeof(double);
        public static readonly Type Boolean = typeof(bool);
        public static readonly Type Null = typeof(object);
        public static readonly Type Undefined = typeof(Undefined);
        public static readonly Type Object = typeof(JsObj);
        public static readonly Type Dynamic = typeof(object);
        public static readonly Type Action = typeof(Action);
        public static readonly Type Self = typeof(SelfMarker);

        public static Type CreateFuncType(params Type[] types)
        {
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            var systemCore = assemblies.First(x => x.FullName.StartsWith("System.Core"));

            if (types.Length == 0)
            {
                return systemCore.GetType("System.Action");
            }
            else
            {
                var type = systemCore.GetType("System.Func`" + types.Length);
                return type.MakeGenericType(types);
            }
        }
    }

    public interface INode
    {
        int Line { get; }
        int Column { get; }
        NodeType NodeType { get; }
        Type ExprType { get; }

        Et Generate(EtGenerator etgen);
        Et Generate2(EtGenerator etgen);
        INode Optimize(AstOptimizer astopt);
        Type EvalTypes(params INode[] nodes);
        bool IdenticalTypes(params INode[] nodes);

        string Print();
        void Print(StringBuilder writer, int indent = 0);
    }
}

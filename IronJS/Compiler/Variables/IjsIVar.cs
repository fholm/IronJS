using System;
using System.Linq.Expressions;

namespace IronJS.Compiler
{
    public interface IjsIVar
    {
        Type ExprType { get; }
        ParameterExpression Expr { get; }
    }
}

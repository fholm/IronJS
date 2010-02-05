using System;
using Microsoft.Scripting.Ast;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler
{
    public interface IjsIVar
    {
        Type ExprType { get; }
        ParameterExpression Expr { get; }
    }
}

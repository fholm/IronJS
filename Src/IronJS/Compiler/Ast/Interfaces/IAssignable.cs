#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast {
    public interface IAssignable : INode {
        Expression Assign(Lambda function, Expression value);
    }
}

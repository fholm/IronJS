using System;
using System.Collections.Generic;
using System.Text;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast
{
    public interface INode
    {
        Type Type { get; }
		INode[] Children { get; }
        Expression Compile(Lambda func);
        INode Analyze(Stack<Lambda> stack);
    }
}

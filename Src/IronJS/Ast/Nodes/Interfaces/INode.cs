using System;
using System.Collections.Generic;
using IronJS.Runtime.Jit;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Nodes
{
    public interface INode
    {
        Type Type { get; }
		INode[] Children { get; }
        Expression Compile(JitContext func);
        INode Analyze(Stack<Lambda> stack);
    }
}

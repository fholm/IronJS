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
    using Et = Expression;

    public interface INode
    {
        int Line { get; }
        int Column { get; }
        Type Type { get; }

        Et Compile(Function func);
        INode Analyze(Stack<Function> func);
        void Write(StringBuilder writer, int indent);
    }
}

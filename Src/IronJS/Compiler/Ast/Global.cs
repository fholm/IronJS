using System.Collections.Generic;
using IronJS.Runtime2.Js;
using Microsoft.Scripting.Utils;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast
{
    public class Global : Function
    {
        public Global(INode body)
            : base(null, null, body, null)
        {
            IsGlobalScope = true;
        }

        public Func<IjsClosure, object> Compile()
        {
            return null;
        }

        public Global Analyze()
        {
            Stack<Function> stack = new Stack<Function>();
            stack.Push(this);

            return (Global) Analyze(stack);
        }

        public static Global Create(List<INode> body)
        {
            return new Global(new Block(body, null));
        }
    }
}

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
    public class GlobalScope : Function
    {
        public GlobalScope(INode body)
            : base(null, null, body, null)
        {
            IsGlobalScope = true;
        }

        public Func<IjsClosure, object> Compile()
        {
            return null;
        }

        public GlobalScope Analyze()
        {
            Stack<Function> stack = new Stack<Function>();
            stack.Push(this);

            return (GlobalScope) Analyze(stack);
        }

        public static GlobalScope Create(List<INode> body)
        {
            return new GlobalScope(new Block(body, null));
        }
    }
}

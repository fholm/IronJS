using System;
using System.Collections.Generic;
using Antlr.Runtime.Tree;
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
        public GlobalScope(Variable name, IEnumerable<string> parameters, INode body, ITree node)
            : base(name, parameters, body, node)
        {
            Globals = Locals;
        }

        public Func<IjsClosure, object> Compile()
        {
            Func<bool> guard;
            return Compile<Func<IjsClosure, object>, Func<bool>>(Type.EmptyTypes, out guard);
        }

        public GlobalScope Analyze()
        {
            return (GlobalScope) Analyze(this);
        }

        public static GlobalScope Create(List<INode> body)
        {
            return new GlobalScope(
                null, null, new Block(body, null), null
            );
        }
    }
}

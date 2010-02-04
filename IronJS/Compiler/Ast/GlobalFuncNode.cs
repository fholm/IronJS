using System;
using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;

namespace IronJS.Compiler.Ast
{
    public class GlobalFuncNode : FuncNode
    {
        public GlobalFuncNode(IdentifierNode name, IEnumerable<string> parameters, INode body, ITree node)
            : base(name, parameters, body, node)
        {
            Globals = Locals;
        }

        public Func<IjsClosure, object> Compile()
        {
            Func<bool> guard;
            return Compile<Func<IjsClosure, object>, Func<bool>>(Type.EmptyTypes, out guard);
        }

        public GlobalFuncNode Analyze()
        {
            return (GlobalFuncNode) Analyze(this);
        }

        public static GlobalFuncNode Create(List<INode> body)
        {
            return new GlobalFuncNode(
                null, null, new BlockNode(body, null), null
            );
        }
    }
}

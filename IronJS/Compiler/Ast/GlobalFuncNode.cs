using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;
using IronJS.Runtime2.Js;
using IronJS.Runtime2.Js;

namespace IronJS.Compiler.Ast
{
    public class GlobalFuncNode : FuncNode
    {
        public GlobalFuncNode(IdentifierNode name, List<IdentifierNode> parameters, INode body, ITree node)
            : base(name, parameters, body, node)
        {

        }

        public Func<IjsClosure, object> Compile()
        {
            return (Func<IjsClosure, object>) Compile(typeof(IjsClosure));
        }

        public GlobalFuncNode Analyze()
        {
            return (GlobalFuncNode) Analyze(this);
        }

        public static GlobalFuncNode Create(List<INode> body)
        {
            return new GlobalFuncNode(
                null,
                new List<IdentifierNode>(),
                new BlockNode(body, null),
                null
            ) {
                IsGlobalScope = true
            };
        }
    }
}

using System;
using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Js;
using System.Text;
using IronJS.Compiler.Tools;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = Expression;

    public class New : Node
    {
        public INode Target { get; protected set; }
        public List<INode> Args { get; protected set; }

        public New(INode target, List<INode> args, ITree node)
            : base(NodeType.New, node)
        {
            Target = target;
            Args = args;
        }

        public New(INode target, ITree node)
            : this(target, new List<INode>(), node)
        {

        }

        public override Type Type
        {
            get
            {
                return IjsTypes.Object;
            }
        }

        public override INode Analyze(Stack<Function> stack)
        {
			AnalyzeTools.IfIdentiferUsedAs(
                Target = Target.Analyze(stack), 
                IjsTypes.Object
            );

            for (int index = 0; index < Args.Count; ++index)
                Args[index] = Args[index].Analyze(stack);

            return this;
        }
    }
}

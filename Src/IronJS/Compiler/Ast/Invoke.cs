using System;
using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Binders;
using IronJS.Runtime2.Js;
using IronJS.Tools;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;
using System.Text;
using IronJS.Compiler.Tools;
using System.Dynamic;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = Expression;
    using EtParam = ParameterExpression;

    public class Invoke : Node
    {
        public INode Target { get; protected set; }
        public List<INode> Args { get; protected set; }

        public Invoke(INode target, List<INode> args, ITree node)
            : base(NodeType.Call, node)
        {
            Target = target;
            Args = args;
        }

        public override INode Analyze(Stack<Function> stack)
        {
            Target = Target.Analyze(stack);

            for (int index = 0; index < Args.Count; ++index)
                Args[index] = Args[index].Analyze(stack);

			AnalyzeTools.IfIdentiferUsedAs(Target, IjsTypes.Object);

            return this;
        }

        public override Et Compile(Function func)
        {
			if (Args.Count == 0)
				return IjsAstTools.Call0(func, Target);

			return IjsAstTools.CallN(func, Target, Args);
        }
    }
}

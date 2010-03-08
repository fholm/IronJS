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
		public INode Target { get { return Children[0]; } }

        public Invoke(INode target, List<INode> args, ITree node)
            : base(NodeType.Call, node)
        {
			Children = new INode[args.Count + 1];
			Children[0] = target;
			args.CopyTo(Children, 1);
        }

        public override INode Analyze(Stack<Function> stack)
        {
			base.Analyze(stack);

			AnalyzeTools.IfIdentiferUsedAs(Target, IjsTypes.Object);

            return this;
        }

        public override Et Compile(Function func)
        {
			var args = ArrayUtils.RemoveFirst(Children);

			if (args.Length == 0)
				return CompileTools.Call0(func, Target);

			return CompileTools.CallN(func, Target, args);
        }
    }
}

using System;
using System.Collections.Generic;
using IronJS.Runtime.Jit;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Nodes {
	public interface INode {
		string Debug { get; }
		Type Type { get; }
		INode[] Children { get; }
		Expression Compile(Lambda func);
		INode Analyze(Stack<Lambda> stack);
	}
}

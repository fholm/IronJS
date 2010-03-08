using System;
using System.Collections.Generic;
using System.Text;
using IronJS.Compiler;
using IronJS.Compiler.Ast;
using IronJS.Runtime.Utils;
using IronJS.Runtime2.Js;
using Microsoft.Scripting.Utils;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Testing {

	using IronJS.Compiler.Tools;

	class Program {
		public static void Main(string[] args) {
			IjsAstGenerator astGenerator = new IjsAstGenerator();
			List<INode> astNodes = astGenerator.Build("Testing.js", Encoding.UTF8);
			GlobalScope globalScope = GlobalScope.Create(astNodes).Analyze();

			DisplayTools.Print(globalScope);

			IjsContext context = new IjsContext();
			Func<IjsClosure, object> compiled = globalScope.Compile(context);

			context.GlobalScope.Set("time", new Action<IjsFunc>(HelperFunctions.Timer));
			context.GlobalScope.Set("print", new Func<object, object>(HelperFunctions.PrintLine));

			object result = compiled(context.GlobalClosure);
		}
	}
}

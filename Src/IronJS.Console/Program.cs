using System;
using System.Collections.Generic;
using System.Text;
using IronJS.Ast;
using IronJS.Ast.Nodes;
using IronJS.Ast.Tools;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Microsoft.Scripting.Utils;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Testing {

	class Program {
		public static void Main(string[] args) {
			Generator astGenerator = new Generator();
			List<INode> astNodes = astGenerator.Build("Testing.js", Encoding.UTF8);
			File file = File.Create(astNodes).Analyze();

			DisplayTools.Print(file);

			Context context = new Context();
			Func<Closure, object> compiled = file.Compile(context);

			context.GlobalScope.Set("print", new Func<object, object>(HelperFunctions.PrintLine));

			object result = compiled(context.GlobalClosure);
		}
	}
}

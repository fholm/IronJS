using System;
using System.Collections.Generic;
using System.Text;
using IronJS.Compiler;
using IronJS.Compiler.Ast;
using IronJS.Runtime.Utils;
using IronJS.Runtime2.Js;
using IronJS.Tools;
using System.Runtime.CompilerServices;

#if CLR2
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Testing {

    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = Expression;
    using EtParam = ParameterExpression;

	public class TestVar : Et {
		public Type type;

		public TestVar()
			: base() {

		}

		protected override Expression Accept(ExpressionVisitor visitor)
		{
			return base.Accept(visitor);
		}

		public override bool CanReduce {
			get {
				return true;
			}
		}

		public override ExpressionType NodeType {
			get {
				return ExpressionType.Extension;
			}
		}

		public override Type Type {
			get {
				return type;
			}
		}

		int _test = 0;
		public override Expression Reduce() {
			return Et.Constant(++_test, typeof(object));
		}
	}



    class Program {
        public static void Main(string[] args) {
			var x = new TestVar();
			x.type = typeof(object);

			var v1 = Et.Variable(typeof(object), "_v1_");
			var v2 = Et.Variable(typeof(object), "_v2_");

			var wline = typeof(Console).GetMethod("WriteLine", new []{ typeof(object) });

			var block = Et.Block(
				new[] { v1, v2 },
				Et.Assign(v1, x),
				Et.Assign(v2, x),
				Et.Call(wline, v1),
				Et.Call(wline, v2)
			);

			var lambda = Et.Lambda(block);

			lambda.Compile().DynamicInvoke();

			/*

            IjsAstGenerator astGenerator = new IjsAstGenerator();
            List<INode> astNodes = astGenerator.Build("Testing.js", Encoding.UTF8);
            GlobalScope globalScope = GlobalScope.Create(astNodes).Analyze();
            Console.WriteLine(globalScope.Print());
            Console.ReadLine();

            IjsContext context = new IjsContext();
            Func<IjsClosure, object> compiled = globalScope.Compile(context);

            context.GlobalScope.Set("time", new Action<IjsFunc>(HelperFunctions.Timer));
            context.GlobalScope.Set("print", new Func<object, object>(HelperFunctions.PrintLine));

            object result = compiled(context.GlobalClosure);
			*/
			}
    }
}

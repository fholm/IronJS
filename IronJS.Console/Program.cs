using System;
using System.Text;
using IronJS.Runtime;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Testing
{
    class Program
    {
        public static void Main(string[] args)
        {
            var astGenerator = new Compiler.AstGenerator();
            var astOptimizer = new Compiler.AstAnalyzer();

            var astNodes = astGenerator.Build("Testing.js", Encoding.UTF8);
                astNodes = astOptimizer.Optimize(astNodes);

            foreach (var node in astNodes)
                Console.WriteLine(node.Print());

            var globals = new IjsObj();

            globals.Set(
                "time", 
                typeof(HelperFunctions).GetMethod("Timer")
            );

            globals.Set(
                "print",
                typeof(Console).GetMethod("WriteLine", new[] { typeof(int) })
            );

            var context = new Compiler.IjsContext();
            var etGenerator = new Compiler.IjsEtGenerator();
            var compiled = etGenerator.Generate(astNodes, context);

            compiled.GetMethod("func").Invoke(null, new[] { (object) globals });
        }
    }
}

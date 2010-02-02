using System;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using IronJS.Extensions;
using IronJS.Runtime.Js;
using Microsoft.Scripting.Generation;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Testing
{
    class Program
    {
        public static void Main(string[] args)
        {
            var astGenerator = new Compiler.IjsAstGenerator();
            var astOptimizer = new Compiler.IjsAstAnalyzer();

            var astNodes = astGenerator.Build("Testing.js", Encoding.UTF8);
                astNodes = astOptimizer.Optimize(astNodes);

            Console.WriteLine(astNodes.PrettyPrint());

            Console.ReadLine();
            return;

            var globals = new IjsObj();

            globals.Set(
                "print",
                typeof(Console).GetMethod("WriteLine", new[] { typeof(int) })
            );

            var context = new Compiler.IjsContext();
            var etGenerator = new Compiler.IjsEtGenerator();
            var compiled = etGenerator.Generate(astNodes, context);

            try
            {
                //compiled.Invoke(null, new[] { (object)globals });
            }
            catch(Exception ex)
            {
                return;
            }
        }
    }
}

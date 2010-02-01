using System;
using System.Linq;
using System.Text;
using IronJS.Extensions;
using IronJS.Runtime.Js;
using System.IO;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Testing
{
    class Program
    {
        public static void Method()
        {

        }

        public static Action Test(Action act)
        {
            return act;
        }

        public static void Main(string[] args)
        {

            var x = Et.Lambda<Action>(
                    Et.Default(typeof(object))
                ).Compile();


            Action deleg = delegate() { };
            Console.WriteLine(IronJS.Utils.Benchmark.This(() =>
            {
                for (int i = 0; i < 1000000; ++i) {
                    /*
                    deleg.Invoke();
                    deleg.Invoke();
                    deleg.Invoke();
                    deleg.Invoke();
                    deleg.Invoke();
                    deleg.Invoke();
                    deleg.Invoke();
                    deleg.Invoke();
                    deleg.Invoke();
                    deleg.Invoke();
                    */
                    /*
                    deleg();
                    deleg();
                    deleg();
                    deleg();
                    deleg();
                    deleg();
                    deleg();
                    deleg();
                    deleg();
                    deleg();
                    */
                    /*
                    x();
                    x();
                    x();
                    x();
                    x();
                    x();
                    x();
                    x();
                    x();
                    x();
                    */
                }
            }).Take(3).Average());

            Console.ReadLine();

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

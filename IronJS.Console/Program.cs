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
        public static void Method(int x)
        {

        }

        public static Action Test(Action act)
        {
            return act;
        }

        public class Cell
        {
            public static Cell<T> Create<T>(T value)
            {
                return new Cell<T>(value);
            }
        }

        public class Cell<T>
        {
            public T Value;

            public Cell(T value)
            {
                Value = value;
            }
        }

        public static void Main(string[] args)
        {
            var param = Et.Parameter(typeof(object), "#tmp");

            var x = Et.Lambda<Action<object>>(
                    Et.Default(typeof(object)),
                    param
                ).Compile();


            var y = CompilerHelpers.CompileToMethod<Action>(
                Et.Lambda<Action>(
                    Et.Default(typeof(object))
                ),
                DebugInfoGenerator.CreatePdbGenerator(),
                false
            );

            var x2 = Cell.Create(1);
            Action deleg = delegate() { };
            Console.WriteLine(IronJS.Utils.Benchmark.This(() =>
            {
                for (int i = 0; i < 1000000; ++i) {
                    x2 = Cell.Create(x2.Value * x2.Value);
                    x2 = Cell.Create(x2.Value * x2.Value);
                    x2 = Cell.Create(x2.Value * x2.Value);
                    x2 = Cell.Create(x2.Value * x2.Value);
                    x2 = Cell.Create(x2.Value * x2.Value);
                    x2 = Cell.Create(x2.Value * x2.Value);
                    x2 = Cell.Create(x2.Value * x2.Value);
                    x2 = Cell.Create(x2.Value * x2.Value);
                    x2 = Cell.Create(x2.Value * x2.Value);
                    x2 = Cell.Create(x2.Value * x2.Value);
                    /*
                    x(x2);
                    x(x2);
                    x(x2);
                    x(x2);
                    x(x2);
                    x(x2);
                    x(x2);
                    x(x2);
                    x(x2);
                    x(x2);
                    */
                    /*
                    Method((int)z);
                    Method((int)z);
                    Method((int)z);
                    Method((int)z);
                    Method((int)z);
                    Method((int)z);
                    Method((int)z);
                    Method((int)z);
                    Method((int)z);
                    Method((int)z);
                    */
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
                    y();
                    y();
                    y();
                    y();
                    y();
                    y();
                    y();
                    y();
                    y();
                    y();
                    */
                    /*
                    Method();
                    Method();
                    Method();
                    Method();
                    Method();
                    Method();
                    Method();
                    Method();
                    Method();
                    Method();
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

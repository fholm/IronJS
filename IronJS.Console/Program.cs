using System;
using System.Text;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using System.Collections.Generic;

namespace IronJS.Testing
{
    class Program
    {
        public static void Main(string[] args)
        {
            var arr = new object[10];
            var start = DateTime.Now;
            var x = 10;

            for (int i = 0; i < 10000000; ++i)
            {
                if (x == 10)
                    x = 10;
            }

            var stop = DateTime.Now;
            var total = stop.Subtract(start);
            Console.WriteLine(total.TotalMilliseconds);

            var dict = new Dictionary<string, object>();
            dict.Add("lo2l", 0);
            dict.Add("lo12l", 0);
            dict.Add("lol23", 0);
            dict.Add("lo12l2", 0); 
            dict.Add("lol324", 0);
            dict.Add("l5ol2", 0); 
            dict.Add("lol2353", 0);
            dict.Add("lol232", 0); 
            dict.Add("lol235", 0);
            dict.Add("lol432", 0);

            start = DateTime.Now;
            bool obj = true;
            for (int j = 0; j < 10000000; ++j)
            {
                if (obj)
                    obj = true;
            }

            stop = DateTime.Now;
            total = stop.Subtract(start);
            Console.WriteLine(total.TotalMilliseconds);

            var astGenerator = new Compiler.IjsAstGenerator();
            var astOptimizer = new Compiler.IjsAstAnalyzer();

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

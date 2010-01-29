using System;
using System.Text;
using IronJS.Runtime;
using IronJS.Runtime.Js;

namespace IronJS.Testing
{
    class Program
    {
        //TODO: fix pretty-print of AST tree for all nodes
        static void Main(string[] args)
        {
            var astGenerator = new Compiler.AstGenerator();
            var astOptimizer = new Compiler.AstOptimizer();

            var astNodes = astGenerator.Build("Testing.js", Encoding.UTF8);
                astNodes = astOptimizer.Optimize(astNodes);

            foreach (var node in astNodes)
                Console.WriteLine(node.Print());

            var etGenerator = new Compiler.EtGenerator();
            var compiled = etGenerator.Build2(astNodes, new Context());
            compiled(new JsObj());

            Console.ReadLine();
        }
    }
}

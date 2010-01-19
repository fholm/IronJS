using System;
using System.Text;
using IronJS.Runtime;
using IronJS.Runtime.Js;
using System.Collections.Generic;

namespace IronJS.Testing
{
    class Program
    {
        //TODO: fix pretty-print of AST tree for all nodes
        static void Main(string[] args)
        {
            var context = Context.Setup();
            var astBuilder = new Compiler.AstGenerator();
            var etGenerator = new Compiler.EtGenerator();

            var astNodes = astBuilder.Build("Testing.js", Encoding.UTF8);

            foreach (var node in astNodes)
                Console.WriteLine(node.Print());

            var compiled = etGenerator.Build(astNodes, context);

            var globals = Scope.CreateGlobal(context);

            context.Setup(globals);

            globals.Global(
                "println",
                typeof(BuiltIns).GetMethod("PrintLine")
            );

            compiled(globals);
        }
    }
}

using System;
using System.Text;
using IronJS.Runtime;
using IronJS.Runtime.Js;
using System.Collections.Generic;

namespace IronJS.Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            var context = Context.Setup();
            var astBuilder = new Compiler.Ast.AstGenerator();
            var etGenerator = new Compiler.EtGenerator();

            var astNodes = astBuilder.Build("IronJS.js", Encoding.UTF8);

            foreach (var node in astNodes)
                Console.WriteLine(node.Print());

            var compiled = etGenerator.Build(astNodes, context);
            var result = compiled.Run(globals => {
                globals.Put(
                    "print",
                    typeof(IronJS.Runtime.BuiltIns).GetMethod("Print")
                );

                globals.Put(
                    "println",
                    typeof(IronJS.Runtime.BuiltIns).GetMethod("PrintLine")
                );

                globals.Put(
                    "exc",
                    new Exception()
                );

                globals.Put(
                    "test",
                    context.CreateArray()
                );
            });
        }
    }
}

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
                globals.Push(
                    "print",
                    typeof(IronJS.Runtime.BuiltIns).GetMethod("Print"),
                    VarType.Global
                );

                globals.Push(
                    "println",
                    typeof(IronJS.Runtime.BuiltIns).GetMethod("PrintLine"),
                    VarType.Global
                );

                globals.Push(
                    "exc",
                    new Exception(),
                    VarType.Global
                );

                globals.Push(
                    "test",
                    context.CreateArray(),
                    VarType.Global
                );
            });
        }
    }
}

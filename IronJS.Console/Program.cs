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
            var astBuilder = new Compiler.Ast.AstGenerator();
            var etGenerator = new Compiler.EtGenerator();

            var astNodes = astBuilder.Build("IronJS.js", Encoding.UTF8);

            foreach (var node in astNodes)
                Console.WriteLine(node.Print());

            Context context = Context.Setup();

            context.SuperGlobals.Push(
                "print", 
                typeof(IronJS.Runtime.BuiltIns).GetMethod("Print"), 
                VarType.Global
            );

            context.SuperGlobals.Push(
                "println", 
                typeof(IronJS.Runtime.BuiltIns).GetMethod("PrintLine"),
                VarType.Global
            );

            context.SuperGlobals.Push(
                "exc", 
                new Exception(),
                VarType.Global
            );

            var compiled = etGenerator.Build(astNodes, context);
            var globals = compiled.Run();
        }
    }
}

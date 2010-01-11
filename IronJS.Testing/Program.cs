using System;
using System.Text;
using IronJS.Runtime;
using IronJS.Runtime.Js;

namespace IronJS.Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            var astBuilder = new Compiler.Ast.AstGenerator();
            var etGenerator = new Compiler.EtGenerator();

            var astNodes = astBuilder.Build("IronJS.js", Encoding.UTF8);

            foreach (var node in astNodes)
                Console.WriteLine(node.Print());

            Context context = Context.Setup();

            context.Globals.Push(
                "print", 
                typeof(IronJS.Runtime.BuiltIns).GetMethod("Print"), 
                VarType.Global
            );

            context.Globals.Push(
                "exc",
                new Exception(),
                VarType.Global
            );

            var compiled = etGenerator.Build(astNodes);

            compiled(context.Globals);
        }
    }
}

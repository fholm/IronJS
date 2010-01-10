using System;
using System.Text;
using System.Linq.Expressions;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using IronJS.Runtime;

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

            Console.WriteLine(((double)1 == (double)1));

            var compiled = etGenerator.Build(astNodes);

            compiled(context.Globals);
        }
    }
}

using System;
using System.Text;
using System.Linq.Expressions;
using IronJS.Runtime.Js;

namespace IronJS.Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            var astBuilder = new Compiler.Ast.Builder();
            var etGenerator = new Compiler.EtGenerator();

            var astNodes = astBuilder.Build("IronJS.js", Encoding.UTF8);

            foreach (var node in astNodes)
                Console.WriteLine(node.Print());

            Frame globalFrame = new Frame();

            globalFrame.Push(
                "print", 
                typeof(IronJS.Runtime.BuiltIns).GetMethod("Print"), 
                VarType.Global
            );

            var compiled = etGenerator.Build(astNodes);

            compiled(globalFrame);
        }
    }
}

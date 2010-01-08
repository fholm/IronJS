using System;
using System.Text;
using System.Linq.Expressions;
using System.Reflection.Emit;
using IronJS.Runtime.Js;

namespace IronJS.Testing
{
    class Program
    {
        static void Main(string[] args)
        {
            var astBuilder = new Compiler.Ast.Builder();
            var treeGenerator = new Compiler.Tree.Generator();
            var context = new Runtime.Context();

            var astNodes = astBuilder.Build("IronJS.js", Encoding.UTF8);

            foreach (var node in astNodes)
                Console.WriteLine(node.Print());

            Frame<Function> funcTable;
            Frame<object> globalScope = new Frame<object>(null);

            globalScope.Push("print", typeof(IronJS.Runtime.BuiltIns).GetMethod("Print"));

            var compiled = treeGenerator.Build(astNodes, out funcTable);

            compiled(funcTable, globalScope);
        }
    }
}

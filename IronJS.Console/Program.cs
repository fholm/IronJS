using System;
using System.Text;
using IronJS.Compiler.Ast;
using IronJS.Runtime2.Js;

namespace IronJS.Testing
{
    class Program
    {
        public static void Main(string[] args)
        {
            var astGenerator = new Compiler.IjsAstGenerator();
            var astNodes = astGenerator.Build("Testing.js", Encoding.UTF8);
            var globalScope = GlobalFuncNode.Create(astNodes).Analyze();

            Console.WriteLine(globalScope.Print());

            var compiled = globalScope.Compile();
            var closure = new IjsClosure(new IjsObj());

            compiled(closure);
        }
    }
}

using System;
using System.Text;
using IronJS.Compiler.Ast;
using IronJS.Runtime2.Js;
using IronJS.Runtime.Utils;
using Microsoft.Scripting.Utils;

namespace IronJS.Testing
{
    class Program
    {
        public static void Main(string[] args)
        {
            var astGenerator = new Compiler.IjsAstGenerator();
            var astNodes = astGenerator.Build("Testing.js", Encoding.UTF8);
            var globalScope = GlobalFuncNode.Create(astNodes).Analyze();


            Console.WriteLine("Welcome to IronJS 0.0.2, Runtime: " + System.Environment.Version);
            Console.Write(">>> ");
            Console.ReadLine();

            Console.WriteLine(globalScope.Print());

            var compiled = globalScope.Compile();
            var closure = new IjsClosure(new IjsObj());

            closure.Globals.Set("time", new Action<IjsFunc>(HelperFunctions.Timer));
            closure.Globals.Set("print", new Func<object, object>(HelperFunctions.PrintLine));

            compiled(closure);

            Console.ReadLine();
        }
    }
}

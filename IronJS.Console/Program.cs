using System;
using System.Text;
using IronJS.Compiler.Ast;
using IronJS.Runtime2.Js;
using IronJS.Runtime.Utils;
using Microsoft.Scripting.Utils;
using IronJS.Compiler;
using System.Collections.Generic;

namespace IronJS.Testing
{
    class Program
    {
        public static void Main(string[] args)
        {
            IjsAstGenerator astGenerator = new IjsAstGenerator();
            List<INode> astNodes = astGenerator.Build("Testing.js", Encoding.UTF8);
            GlobalFuncNode globalScope = GlobalFuncNode.Create(astNodes).Analyze();


            Console.WriteLine("Welcome to IronJS 0.0.2, Runtime: " + System.Environment.Version);
            Console.Write(">>> ");
            Console.ReadLine();

            Console.WriteLine(globalScope.Print());

            Func<IjsClosure, object> compiled = globalScope.Compile();
            IjsClosure closure = new IjsClosure(new IjsObj());

            closure.Globals.Set("time", new Action<IjsFunc>(HelperFunctions.Timer));
            closure.Globals.Set("print", new Func<object, object>(HelperFunctions.PrintLine));

            compiled(closure);

            Console.ReadLine();
        }
    }
}

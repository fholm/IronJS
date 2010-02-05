using System;
using System.Collections.Generic;
using System.Text;
using IronJS.Compiler;
using IronJS.Compiler.Ast;
using IronJS.Runtime.Utils;
using IronJS.Runtime2.Js;
using Microsoft.Scripting.Utils;

namespace IronJS.Testing
{
    class Program
    {
        public static void Main(string[] args)
        {
            IjsAstGenerator astGenerator = new IjsAstGenerator();

            List<INode> astNodes = astGenerator.Build("Testing.js", Encoding.UTF8);
            GlobalFuncNode globalScope = GlobalFuncNode.Create(astNodes).Analyze();
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

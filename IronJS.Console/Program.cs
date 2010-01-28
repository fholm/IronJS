using System.Text;
using IronJS.Runtime;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using System;
using Microsoft.Scripting.Utils;

namespace IronJS.Testing
{
    class Program
    {
        //TODO: fix pretty-print of AST tree for all nodes
        static void Main(string[] args)
        {
            arr = ArrayUtils.RemoveFirst(arr);
            arr = ArrayUtils.RemoveFirst(arr);

            var context = new Context();
            var astBuilder = new Compiler.AstGenerator();
            var etGenerator = new Compiler.EtGenerator();
            var astNodes = astBuilder.Build("Testing.js", Encoding.UTF8);
            var compiled = etGenerator.Build(astNodes, context);
            var globals = Scope.CreateGlobal(context);

            context.SetupGlobals(globals);

            globals.Global(
                "println",
                typeof(HelperFunctions).GetMethod("PrintLine")
            );

            compiled(globals);
        }
    }
}

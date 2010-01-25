using System;
using System.Text;
using IronJS.Runtime;
using IronJS.Runtime.Js;
using System.Collections.Generic;
using IronJS.Runtime.Utils;

namespace IronJS.Testing
{
    class Program
    {

        //TODO: fix pretty-print of AST tree for all nodes
        static void Main(string[] args)
        {
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

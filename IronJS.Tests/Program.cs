using System.Text;
using IronJS.Runtime;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using System.Reflection.Emit;
using System;

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
            var obj = new JsObj();
            compiled.Invoke(null, new[] { obj });
        }
    }
}

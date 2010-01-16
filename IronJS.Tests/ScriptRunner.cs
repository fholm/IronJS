using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Tests
{
    static class ScriptRunner
    {
        static public string Run(string source)
        {
            var emitter = new StringBuilder();
            var context = Runtime.Context.Setup();

            context.SuperGlobals.Push(
                "emit",
                new Func<object, StringBuilder>(emitter.Append),
                Runtime.Js.VarType.Local
            );

            var astBuilder = new Compiler.Ast.AstGenerator();
            var etGenerator = new Compiler.EtGenerator();

            var astNodes = astBuilder.Build(source);
            var compiled = etGenerator.Build(astNodes, context);

            var globals = compiled.Run();
            return emitter.ToString();
        }

        static public string RunFile(string filename)
        {
            var emitter = new StringBuilder();
            var context = Runtime.Context.Setup();

            context.SuperGlobals.Push(
                "emit",
                new Func<object, StringBuilder>(emitter.Append),
                Runtime.Js.VarType.Local
            );

            var astBuilder = new Compiler.Ast.AstGenerator();
            var etGenerator = new Compiler.EtGenerator();

            var astNodes = astBuilder.Build(filename, Encoding.UTF8);
            var compiled = etGenerator.Build(astNodes, context);

            var globals = compiled.Run();
            return emitter.ToString();
        }
    }
}

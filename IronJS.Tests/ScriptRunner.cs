using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Tests
{
    static class ScriptRunner
    {
        static public string Run(string source)
        {
            var emitter = new StringBuilder();
            var context = Runtime.Context.Setup();

            var astBuilder = new Compiler.AstGenerator();
            var etGenerator = new Compiler.EtGenerator();

            var scope = Scope.CreateGlobal(context);

            Func<object, object> emit = (obj) => {
                return emitter.Append(JsTypeConverter.ToString(obj));
            };

            scope.Global(
                "emit",
                emit
            );

            context.Setup(scope);

            var astNodes = astBuilder.Build(source);
            var compiled = etGenerator.Build(astNodes, context);

            compiled(scope);

            return emitter.ToString();
        }

        static public string RunFile(string filename)
        {
            var emitter = new StringBuilder();
            var context = Runtime.Context.Setup();

            var astBuilder = new Compiler.AstGenerator();
            var etGenerator = new Compiler.EtGenerator();

            var scope = Scope.CreateGlobal(context);
            
            Func<object, object> emit = (obj) => {
                return emitter.Append(JsTypeConverter.ToString(obj));
            };

            scope.Global(
                "emit",
                emit
            );

            context.Setup(scope);

            var astNodes = astBuilder.Build(filename, Encoding.UTF8);
            var compiled = etGenerator.Build(astNodes, context);

            compiled(scope);

            return emitter.ToString();
        }
    }
}

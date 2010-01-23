using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

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
            scope.Global("emit", emit);

            Action<object> assert = (obj) => {
                Assert.AreEqual((object)true, obj);
            };
            scope.Global("assert", assert);

            Action<object, object> assertEqual = (a, b) => {
                Assert.AreEqual(a, b);
            };
            scope.Global("assertEqual", assertEqual);

            context.SetupGlobals(scope);

            var astNodes = astBuilder.Build(source);
            var compiled = etGenerator.Build(astNodes, context);

            compiled(scope);

            return emitter.ToString();
        }

        static public string Run(string source, ref StringBuilder emitter)
        {
            var context = Runtime.Context.Setup();
            var emitter2 = emitter;

            var astBuilder = new Compiler.AstGenerator();
            var etGenerator = new Compiler.EtGenerator();

            var scope = Scope.CreateGlobal(context);

            Func<object, object> emit = (obj) =>
            {
                emitter2.Append(JsTypeConverter.ToString(obj));
                return null;
            };

            scope.Global(
                "emit",
                emit
            );

            context.SetupGlobals(scope);

            var astNodes = astBuilder.Build(source);
            var compiled = etGenerator.Build(astNodes, context);

            compiled(scope);

            return emitter.ToString();
        }
    }
}

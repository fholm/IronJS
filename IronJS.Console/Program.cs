using System;
using System.Text;
using IronJS.Compiler.Ast;
using IronJS.Runtime.Js;

namespace IronJS.Testing
{
    class Program
    {
        public static void Main(string[] args)
        {
            var astGenerator = new Compiler.IjsAstGenerator();
            var astNodes = astGenerator.Build("Testing.js", Encoding.UTF8);
            var globalScope = FuncNode.CreateGlobalScope(astNodes).Analyze();

            Console.WriteLine(globalScope.Print());

            var globals = new IjsObj();

            globals.Set(
                "print",
                typeof(Console).GetMethod("WriteLine", new[] { typeof(int) })
            );

            var etGenerator = new Compiler.IjsEtGenerator();
            var compiled = etGenerator.Generate(globalScope);

            try
            {
                //compiled.Invoke(null, new[] { (object)globals });
            }
            catch(Exception ex)
            {
                return;
            }
        }
    }
}

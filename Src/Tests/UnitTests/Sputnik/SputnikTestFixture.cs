namespace IronJS.Tests.UnitTests.Sputnik
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Text;
    using NUnit.Framework;

    public class SputnikTestFixture
    {
        private static readonly string basePath;
        private static readonly string libPath;
        private readonly string testsPath;

        static SputnikTestFixture()
        {
            var assemblyPath = Program.GetAssemblyPath();
            var assemblyDirectory = new DirectoryInfo(Path.GetDirectoryName(assemblyPath));

            basePath = assemblyDirectory.Parent.Parent.Parent.FullName;
            libPath = Path.Combine(basePath, "Sputnik", "sputnik-v1", "lib");
            basePath = Path.Combine(basePath, "Sputnik", "sputnik-v1", "tests");
        }

        public SputnikTestFixture(string testsPath)
        {
            this.testsPath = Path.Combine(basePath, testsPath);
        }

        private static IronJS.Hosting.CSharp.Context CreateContext(Action<string> errorAction)
        {
            var ctx = new IronJS.Hosting.CSharp.Context();

            Action<string> failAction = error => Assert.Fail(error);
            Action<string> printAction = message => Trace.WriteLine(message);
            Action<string> includeAction = file => ctx.ExecuteFile(Path.Combine(libPath, file));

            var errorFunc = IronJS.Native.Utils.CreateFunction(ctx.Environment, 1, errorAction);
            var failFunc = IronJS.Native.Utils.CreateFunction(ctx.Environment, 1, failAction);
            var printFunc = IronJS.Native.Utils.CreateFunction(ctx.Environment, 1, printAction);
            var includeFunc = IronJS.Native.Utils.CreateFunction(ctx.Environment, 1, includeAction);

            ctx.SetGlobal("$FAIL", failFunc);
            ctx.SetGlobal("ERROR", errorFunc);
            ctx.SetGlobal("$ERROR", errorFunc);
            ctx.SetGlobal("$PRINT", printFunc);
            ctx.SetGlobal("$INCLUDE", includeFunc);

            return ctx;
        }

        public void RunFile(string fileName)
        {
            StringBuilder errorText = new StringBuilder();

            fileName = Path.Combine(this.testsPath, fileName);
            var ctx = CreateContext(e => errorText.AppendLine(e));
            ctx.ExecuteFile(fileName);

            if (errorText.Length > 0)
            {
                Assert.Fail(errorText.ToString());
            }
        }
    }
}
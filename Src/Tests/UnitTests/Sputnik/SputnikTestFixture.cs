namespace IronJS.Tests.UnitTests.Sputnik
{
    using System;
    using System.IO;
    using System.Reflection;
    using NUnit.Framework;

    public class SputnikTestFixture
    {
        private static readonly string basePath;
        private readonly string testsPath;

        static SputnikTestFixture()
        {
            var assemblyPath = Program.GetAssemblyPath();
            var assemblyDirectory = new DirectoryInfo(Path.GetDirectoryName(assemblyPath));

            basePath = assemblyDirectory.Parent.Parent.Parent.FullName;
            basePath = Path.Combine(basePath, "Sputnik", "sputnik-v1", "tests");
        }

        public SputnikTestFixture(string testsPath)
        {
            this.testsPath = Path.Combine(basePath, testsPath);
        }

        private static IronJS.Hosting.CSharp.Context CreateContext()
        {
            var ctx = new IronJS.Hosting.CSharp.Context();

            Action<string> errorAction = error => Assert.Fail(error);
            var errorFunc = IronJS.Native.Utils.createHostFunction(ctx.Environment, errorAction);

            ctx.SetGlobal("ERROR", errorFunc);
            ctx.SetGlobal("$ERROR", errorFunc);
            return ctx;
        }

        public void RunFile(string fileName)
        {
            fileName = Path.Combine(this.testsPath, fileName);
            var ctx = CreateContext();
            ctx.ExecuteFile(fileName);
        }

        public void RunFile_ExpectException<T>(string fileName) where T : Exception
        {
            Assert.Throws<T>(() => RunFile(fileName));
        }

        public void RunFile_ExpectException(string fileName)
        {
            RunFile_ExpectException<Exception>(fileName);
        }
    }
}
namespace IronJS.Tests.UnitTests.Sputnik
{
    using System;
    using System.IO;
    using System.Reflection;
    using Microsoft.VisualStudio.TestTools.UnitTesting;

    public class SputnikTestFixture
    {
        private static readonly string basePath;
        private readonly string testsPath;

        static SputnikTestFixture()
        {
            var assemblyPath = new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
            var assemblyDirectory = new DirectoryInfo(Path.GetDirectoryName(assemblyPath));

            basePath = assemblyDirectory.Parent.Parent.Parent.FullName;
            basePath = Path.Combine(basePath, "Sputnik", "sputnik-v1", "tests");
        }

        public SputnikTestFixture(string testsPath)
        {
            this.testsPath = Path.Combine(basePath, testsPath);
        }

        private static IronJS.Hosting.Context CreateContext()
        {
            var ctx = IronJS.Hosting.Context.Create();

            Action<string> errorAction = error => Assert.Fail(error);
            var errorFunc = IronJS.Native.Utils.createHostFunction(ctx.Environment, errorAction);

            ctx.PutGlobal("ERROR", errorFunc);
            ctx.PutGlobal("$ERROR", errorFunc);
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
            bool pass = false;
            try
            {
                RunFile(fileName);
            }
            catch (T)
            {
                pass = true;
            }

            if (!pass)
            {
                throw new AssertFailedException("Expected exception.");
            }
        }

        public void RunFile_ExpectException(string fileName)
        {
            RunFile_ExpectException<Exception>(fileName);
        }
    }
}

namespace IronJS.Tests.UnitTests.IE9
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using NUnit.Framework;

    public class IE9TestFixture
    {
        private static readonly string basePath;
        private readonly string testsPath;

        static IE9TestFixture()
        {
            var assemblyPath = Program.GetAssemblyPath();
            var assemblyDirectory = new DirectoryInfo(Path.GetDirectoryName(assemblyPath));

            basePath = Path.Combine(assemblyDirectory.Parent.Parent.Parent.FullName, "ietestcenter");
        }

        public IE9TestFixture(string testsPath)
        {
            this.testsPath = Path.Combine(basePath, testsPath);
        }

        private static IronJS.Hosting.CSharp.Context CreateContext()
        {
            var ctx = new IronJS.Hosting.CSharp.Context();

            ctx.Execute("var currentTest; var ES5Harness = { registerTest: function (test) { currentTest = test; } };");

            return ctx;
        }

        public void RunFile(string fileName)
        {
            fileName = Path.Combine(this.testsPath, fileName);
            var ctx = CreateContext();
            ctx.ExecuteFile(fileName);

            var descriptor = ctx.GetGlobal("currentTest").Object;

            var precondition = descriptor.GetT<FunctionObject>("precondition");
            var preconditionFunc = precondition.MetaData.GetDelegate<Func<FunctionObject, CommonObject, BoxedValue>>(precondition);
            var preconditionMet = preconditionFunc.Invoke(precondition, ctx.Globals).Bool;

            Assume.That(preconditionMet);

            var test = descriptor.GetT<FunctionObject>("test");
            var testFunc = test.MetaData.GetDelegate<Func<FunctionObject, CommonObject, BoxedValue>>(test);
            var testPassed = testFunc.Invoke(test, ctx.Globals).Bool;

            Assert.That(testPassed);
        }
    }
}
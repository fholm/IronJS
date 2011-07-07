namespace IronJS.Tests.UnitTests.IE9
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Text;
    using IronJS.Runtime;
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

            Func<BoxedValue, bool> fnExists = f => f.IsFunction;
            var fnExistsFunc = IronJS.Native.Utils.CreateFunction(ctx.Environment, 1, fnExists);
            ctx.SetGlobal("fnExists", fnExistsFunc);

            ctx.Execute("var currentTest; var ES5Harness = { registerTest: function (test) { currentTest = test; } };");

            return ctx;
        }

        public void RunFile(string fileName)
        {
            fileName = Path.Combine(this.testsPath, fileName);
            var ctx = CreateContext();
            ctx.ExecuteFile(fileName);

            var descriptor = ctx.GetGlobal("currentTest").Object;

            var preconditionBoxed = descriptor.Get("precondition");
            if (!preconditionBoxed.IsUndefined)
            {
                var precondition = preconditionBoxed.Unbox<FunctionObject>();
                var preconditionFunc = precondition.MetaData.GetDelegate<Func<FunctionObject, CommonObject, BoxedValue>>(precondition);
                bool preconditionMet;
                try
                {
                    preconditionMet = preconditionFunc.Invoke(precondition, ctx.Globals).Bool;
                }
                catch (Exception ex)
                {
                    Assume.That(false, ex.Message); return;
                }

                Assume.That(preconditionMet);
            }

            var test = descriptor.GetT<FunctionObject>("test");
            var testFunc = test.MetaData.GetDelegate<Func<FunctionObject, CommonObject, BoxedValue>>(test);
            var testPassed = testFunc.Invoke(test, ctx.Globals).Bool;

            Assert.That(testPassed);
        }
    }
}
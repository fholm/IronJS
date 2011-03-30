using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Benchmarks
{
    public class V8BenchMarkTestSuite : TestSuite
    {
        public V8BenchMarkTestSuite(string basePath)
            : base(Path.Combine(basePath, "v8"))
        {
        }

        public override string SuiteName
        {
            get { return "V8 Benchmark Suite - version 6"; }
        }

        public override IEnumerable<string> EnumerateTests()
        {
            return base.EnumerateTests()
                .Where(path => Path.GetFileName(path) != "base.js")
                .Where(path => Path.GetFileName(path) != "run.js");
        }

        protected override IronJS.Hosting.CSharp.Context CreateContext()
        {
            var ctx = base.CreateContext();
            ctx.ExecuteFile(Path.Combine(this.BasePath, "base.js"));
            return ctx;
        }

        protected override string ExecuteTest(IronJS.Hosting.CSharp.Context ctx, string test)
        {
            var errors = string.Empty;

            Action<string> appendError = err =>
            {
                if (!string.IsNullOrEmpty(errors))
                {
                    errors += Environment.NewLine;
                }

                errors += err;
            };

            Action<string, string> printResult = (name, result) => ColorPrint(ConsoleColor.Green, name + ": " + result);
            Action<string, string> printError = (name, error) => appendError(name + ": " + error);
            Action<string> printScore = (score) => ColorPrint(ConsoleColor.Green, "Score: " + score);
            ctx.SetGlobal("PrintResult", IronJS.Native.Utils.createHostFunction(ctx.Environment, printResult));
            ctx.SetGlobal("PrintError", IronJS.Native.Utils.createHostFunction(ctx.Environment, printError));
            ctx.SetGlobal("PrintScore", IronJS.Native.Utils.createHostFunction(ctx.Environment, printScore));

            try
            {
                ctx.ExecuteFile(test);
                ctx.Execute(@"BenchmarkSuite.RunSuites({ NotifyResult: PrintResult,
                                                         NotifyError: PrintError,
                                                         NotifyScore: PrintScore });");
            }
            catch (Exception ex)
            {
                appendError("Exception: " + ex.GetBaseException().Message);
            }

            return errors;
        }
    }
}

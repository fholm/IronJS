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

        protected override TestResult ExecuteTest(IronJS.Hosting.CSharp.Context ctx, string test)
        {
            var errors = new StringBuilder();

            Action<string> appendError = err =>
            {
                if (errors.Length > 0)
                {
                    errors.AppendLine();
                }

                errors.Append(err);
            };

            var score = string.Empty;

            Action<string, string> notifyResult = (name, result) => { };
            Action<string, string> notifyError = (name, error) => appendError(name + ": " + error);
            Action<string> notifyScore = s => score = s;
            ctx.SetGlobal("NotifyResult", IronJS.Native.Utils.createHostFunction(ctx.Environment, notifyResult));
            ctx.SetGlobal("NotifyError", IronJS.Native.Utils.createHostFunction(ctx.Environment, notifyError));
            ctx.SetGlobal("NotifyScore", IronJS.Native.Utils.createHostFunction(ctx.Environment, notifyScore));

            try
            {
                ctx.ExecuteFile(test);
                ctx.Execute(@"BenchmarkSuite.RunSuites({ NotifyResult: NotifyResult,
                                                         NotifyError: NotifyError,
                                                         NotifyScore: NotifyScore });");
            }
            catch (Exception ex)
            {
                appendError("Exception: " + ex.GetBaseException().Message);
            }

            if (errors.Length > 0)
            {
                return new TestResult { Error = errors.ToString() };
            }
            else
            {
                return new TestResult { Score = score };
            }
        }

        protected override TestResult AggregateResults(IList<TestResult> results)
        {
            if (results.Where(r => !string.IsNullOrEmpty(r.Error)).Any())
            {
                return new TestResult { Error = "Could not aggregate the score, because errors exist." };
            }

            return new TestResult { Score = "TODO: Calculate the geometric mean of the results." };
        }
    }
}

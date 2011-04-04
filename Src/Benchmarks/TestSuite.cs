namespace Benchmarks
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using System.Diagnostics;

    public abstract class TestSuite
    {
        public readonly string BasePath;

        public TestSuite(string basePath)
        {
            this.BasePath = basePath;
        }

        public abstract string SuiteName { get; }

        public virtual bool ReuseContext { get { return false; } }

        public virtual IEnumerable<string> EnumerateTests()
        {
            return Directory.GetFiles(this.BasePath, "*.js", SearchOption.AllDirectories);
        }

        protected virtual IronJS.Hosting.CSharp.Context CreateContext()
        {
            return new IronJS.Hosting.CSharp.Context();
        }

        protected void ColorPrint(ConsoleColor color, string value)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(value);
            Console.ForegroundColor = prevColor;
        }

        protected void ColorPrintLine(ConsoleColor color, string value)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(value);
            Console.ForegroundColor = prevColor;
        }

        protected void ColorPrint(ConsoleColor color, object value)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.Write(value);
            Console.ForegroundColor = prevColor;
        }

        protected void ColorPrintLine(ConsoleColor color, object value)
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = color;
            Console.WriteLine(value);
            Console.ForegroundColor = prevColor;
        }

        public void Run()
        {
            var tests = this.EnumerateTests().ToList();
            var results = new List<TestResult>(tests.Count);

            ColorPrintLine(ConsoleColor.DarkCyan, this.SuiteName);
            ColorPrintLine(ConsoleColor.DarkCyan, "==================================");

            foreach (var test in tests)
            {
                Console.WriteLine(Path.GetFileName(test));

                var ctx = this.CreateContext();

                var result = this.ExecuteTest(ctx, test);
                results.Add(result);

                if (!string.IsNullOrEmpty(result.Error))
                {
                    Console.Write("  Error: ");
                    ColorPrintLine(ConsoleColor.Red, result.Error);
                }
                else
                {
                    Console.Write("  Score: ");
                    ColorPrintLine(ConsoleColor.Green, result.Score);
                }
            }

            Console.WriteLine();
            ColorPrintLine(ConsoleColor.DarkCyan, "Whole Suite");

            var suiteResult = this.AggregateResults(results);

            if (!string.IsNullOrEmpty(suiteResult.Error))
            {
                Console.Write("  Error: ");
                ColorPrintLine(ConsoleColor.Red, suiteResult.Error);
            }
            else
            {
                Console.Write("  Score: ");
                ColorPrintLine(ConsoleColor.Green, suiteResult.Score);
            }

            Console.WriteLine();
        }

        protected virtual TestResult ExecuteTest(IronJS.Hosting.CSharp.Context ctx, string test)
        {
            var sw = Stopwatch.StartNew();
            try
            {
                ctx.ExecuteFile(test);
                sw.Stop();
            }
            catch (Exception ex)
            {
                return new TestResult { Error = ex.GetBaseException().Message };
            }

            return new TestResult { Score = sw.ElapsedMilliseconds + "ms" };
        }

        protected abstract TestResult AggregateResults(IList<TestResult> results);
    }
}

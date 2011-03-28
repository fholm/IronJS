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

        protected virtual IronJS.Hosting.Context CreateContext()
        {
            return IronJS.Hosting.Context.Create();
        }

        public void Run()
        {
            var tests = this.EnumerateTests().ToList();

            Console.WriteLine(this.SuiteName);
            Console.WriteLine("==================================");

            foreach (var test in tests)
            {
                Console.Write(Path.GetFileName(test) + ": ");

                var ctx = this.CreateContext();

                var error = this.ExecuteTest(ctx, test);

                if (!string.IsNullOrEmpty(error))
                {
                    var prevColor = Console.ForegroundColor;
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(error);
                    Console.ForegroundColor = prevColor;
                }
            }

            Console.WriteLine();
        }

        protected virtual string ExecuteTest(IronJS.Hosting.Context ctx, string test)
        {
            try
            {
                var sw = Stopwatch.StartNew();
                ctx.ExecuteFile(test);
                sw.Stop();

                Console.WriteLine(sw.ElapsedMilliseconds + "ms");
            }
            catch (Exception ex)
            {
                return "Exception: " + ex.GetBaseException().Message;
            }

            return null;
        }
    }
}

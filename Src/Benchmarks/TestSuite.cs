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

            ColorPrintLine(ConsoleColor.DarkCyan, this.SuiteName);
            ColorPrintLine(ConsoleColor.DarkCyan, "==================================");

            foreach (var test in tests)
            {
                Console.Write(Path.GetFileName(test) + ": ");

                var ctx = this.CreateContext();

                var sw = Stopwatch.StartNew();
                var error = this.ExecuteTest(ctx, test);
                sw.Stop();

                if (!string.IsNullOrEmpty(error))
                {
                    ColorPrintLine(ConsoleColor.Red, error);
                }
                else
                {
                    ColorPrintLine(ConsoleColor.Yellow, sw.ElapsedMilliseconds + "ms");
                }
            }

            Console.WriteLine();
        }

        protected virtual string ExecuteTest(IronJS.Hosting.CSharp.Context ctx, string test)
        {
            try
            {
                ctx.ExecuteFile(test);
            }
            catch (Exception ex)
            {
                return "Exception: " + ex.GetBaseException().Message;
            }

            return null;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.IO;
using System.Diagnostics;

namespace Benchmarks
{
    class Program
    {
        private static string GetExecutableDirectory()
        {
            return Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
        }

        static void Main(string[] args)
        {
            var basePath = new DirectoryInfo(GetExecutableDirectory()).Parent.Parent.FullName;

            //TestSuite sunSpider = new SunSpiderTestSuite(basePath);
            //sunSpider.Run();

            TestSuite v8Benchmark = new V8BenchMarkTestSuite(basePath);
            v8Benchmark.Run();

            if (Debugger.IsAttached)
            {
                Console.ReadKey(true);
            }
        }
    }
}

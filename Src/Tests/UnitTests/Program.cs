namespace IronJS.Tests.UnitTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Reflection;
    using NUnit.Framework;
    using NUnit.ConsoleRunner;

    internal class Program
    {
        [STAThread]
        public static void Main(string[] args)
        {
            Runner.Main(new[] { Program.GetAssemblyPath(), "/noshadow", "/nodots", "/nologo", "/labels" });
        }

        public static string GetAssemblyPath()
        {
            return new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath;
        }
    }
}

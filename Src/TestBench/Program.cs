using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS;
using IronJS.Hosting;

namespace TestBench
{
    class Program
    {
        static void Main(string[] args)
        {
            var ctx = new CSharp.Context();
            dynamic Globals = ctx.Globals;

            Green(() => Console.WriteLine("This is the test bench for testing interop with IronJS"));

            Console.ReadKey(true);
        }

        #region Output Helpers
        private static void Green(Action action)
        {
            Color(ConsoleColor.Green, action);
        }

        private static void Red(Action action)
        {
            Color(ConsoleColor.Red, action);
        }

        private static void Blue(Action action)
        {
            Color(ConsoleColor.Blue, action);
        }

        private static void Yellow(Action action)
        {
            Color(ConsoleColor.Yellow, action);
        }

        private static void Color(ConsoleColor consoleColor, Action action)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = consoleColor;
            action();
            Console.ForegroundColor = originalColor;
        }
        #endregion
    }
}

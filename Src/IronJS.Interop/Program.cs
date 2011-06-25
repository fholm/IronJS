using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Interop
{
    class Program
    {
        static void Main(string[] args)
        {
            var ctx = new IronJS.Hosting.CSharp.Context();
            ctx.CreatePrintFunction();

            Func<FunctionObject, CommonObject, string, CommonObject> require = (func, @this, uriString) =>
            {
                try
                {
                    return TypeLoader.Require(func, @this, uriString);
                }
                catch (UriFormatException ex)
                {
                    ctx.Environment.RaiseReferenceError<object>(ex.Message);
                    throw;
                }
            };

            ctx.SetGlobal("require", Native.Utils.CreateFunction(ctx.Environment, 1, require));
            ctx.Execute(@"
                var a = require('clr:System.IO.File');
                print(a);
            ");

            Console.ReadKey(true);
        }
    }
}

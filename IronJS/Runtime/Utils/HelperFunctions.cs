using System;
using IronJS.Runtime2.Js;
using Microsoft.Scripting.Utils;

namespace IronJS.Runtime.Utils
{
    public static class HelperFunctions
    {
        static public object PrintLine(object obj)
        {
            Console.WriteLine(obj);
            return obj;
        }

        static public void Timer(IjsFunc proxy)
        {
            Func<bool> guard;
            var lambda = proxy.Node.Compile<Func<IjsClosure, object>, Func<bool>>(Type.EmptyTypes, out guard);

            var start = DateTime.Now;
            lambda(proxy.Closure);
            var stop = DateTime.Now;
            var total = stop.Subtract(start);
            Console.WriteLine(total.TotalMilliseconds);
        }
    }
}

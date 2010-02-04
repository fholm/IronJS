using System;
using IronJS.Runtime2.Js;

namespace IronJS.Runtime.Utils
{
    public static class HelperFunctions
    {
        static public object PrintLine(object obj)
        {
            Console.WriteLine(JsTypeConverter.ToString(obj));
            return obj;
        }

        static public void Timer(IjsProxy proxy)
        {
            var lambda = (Func<IjsClosure, object>)proxy.Node.Compile(proxy.ClosureType, Type.EmptyTypes, Type.EmptyTypes).Item2;
            var start = DateTime.Now;
            lambda(proxy.Closure);
            var stop = DateTime.Now;
            var total = stop.Subtract(start);
            Console.WriteLine(total.TotalMilliseconds);
        }
    }
}

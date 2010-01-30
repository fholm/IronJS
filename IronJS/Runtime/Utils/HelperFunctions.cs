using System;
using IronJS.Runtime.Js;

namespace IronJS.Runtime.Utils
{
    public static class HelperFunctions
    {
        static public object PrintLine(object obj)
        {
            Console.WriteLine(JsTypeConverter.ToString(obj));
            return obj;
        }

        static public void Timer(Func<object> func)
        {
            var start = DateTime.Now;
            func();
            var stop = DateTime.Now;
            var total = stop.Subtract(start);
            Console.WriteLine(total.TotalMilliseconds);
        }
    }
}

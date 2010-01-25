using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Runtime.Utils
{
    public static class HelperFunctions
    {
        static public object PrintLine(object obj)
        {
            Console.WriteLine(JsTypeConverter.ToString(obj));
            return obj;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace IronJS.Reflect
{
    static public class Method
    {
        static public MethodInfo GetMethod<T>(string name) where T : class
        {
            return typeof(T).GetMethod(name, Type.EmptyTypes);
        }
    }
}

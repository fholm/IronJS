using System;
using System.Linq;

namespace IronJS.Extensions
{
    public static class TypeExt
    {
        public static string ShortName(this Type that)
        {
            return that.Name.Split('.').Last();
        }
    }
}

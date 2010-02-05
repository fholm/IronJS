using System;
using System.Runtime.CompilerServices;

namespace IronJS.Tools
{
    public static class TypeTools
    {
        public static Type StrongBoxType = typeof(StrongBox<>);

        public static string ShortName(Type that)
        {
            return ArrayTools.Last(that.Name.Split('.'));
        }
    }
}

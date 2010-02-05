using System;

namespace IronJS.Tools
{
    public static class TypeTools
    {
        public static string ShortName(Type that)
        {
            return ArrayTools.Last(that.Name.Split('.'));
        }
    }
}

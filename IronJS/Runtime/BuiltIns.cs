using System;

namespace IronJS.Runtime
{
    public enum ToPrimitiveHint { Number , String }

    public static class BuiltIns
    {
        public static object Add(double left, double right)
        {
            return (double) left + (double) right;
        }

        public static object Print(object o)
        {
            Console.WriteLine(o);
            return o;
        }

  

    }
}

using System;

namespace IronJS.Runtime
{
    public static class BuiltIns
    {
        public static object Add(object left, object right)
        {
            return (double) left + (double) right;
        }

        public static object Print(object o)
        {
            Console.WriteLine(o);
            return o;
        }

        public static object Inspect(Js.Obj arguments, Js.Obj that, object o)
        {
            return o;
        }
    }
}

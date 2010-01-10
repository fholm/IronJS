using System;

namespace IronJS.Runtime
{
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

        public static object TypeOf(object obj)
        {
            if (obj == null)
                return "object";

            if (obj is Js.Undefined)
                return "undefined";

            if (obj is bool)
                return "boolean";

            if (obj is double)
                return "number";

            if (obj is string)
                return "string";

            if (obj is Js.Obj)
            {
                if (((Js.Obj)obj).Class == Js.ObjClass.Function)
                    return "function";

                return "object";
            }

            return "dotnet";
        }
    }
}

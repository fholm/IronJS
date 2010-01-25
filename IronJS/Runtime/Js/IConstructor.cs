using System.Reflection;

namespace IronJS.Runtime.Js
{
    interface IConstructor : IFunction
    {
        IObj Construct(object[] args);  // [[Construct]]
        bool HasInstance(object obj);   // [[HasInstance]]
    }

    static public class IConstructorMethods
    {
        static public readonly MethodInfo MiConstruct = typeof(IFunction).GetMethod("Construct");
        static public readonly MethodInfo MiHasInstance = typeof(IFunction).GetMethod("HasInstance");
    }
}

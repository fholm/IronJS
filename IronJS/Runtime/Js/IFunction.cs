using System.Reflection;

namespace IronJS.Runtime.Js
{
    public interface IFunction : IObj
    {
        // 8.6.2
        object Call(IObj that, object[] args);  // [[Call]]
        IObj Construct(object[] args);          // [[Construct]]
        bool HasInstance(object obj);           // [[HasInstance]]
    }

    static public class IFunctionMethods
    {
        static public readonly MethodInfo MiCall = typeof(IFunction).GetMethod("Call");
        static public readonly MethodInfo MiConstruct = typeof(IFunction).GetMethod("Construct");
        static public readonly MethodInfo MiHasInstance = typeof(IFunction).GetMethod("HasInstance");
    }
}

using System.Reflection;

namespace IronJS.Runtime.Js
{
    public interface IFunction : IObj
    {
        // 8.6.2
        object Call(IObj that, object[] args);  // [[Call]]
    }

    static public class IFunctionMethods
    {
        static public readonly MethodInfo MiCall = typeof(IFunction).GetMethod("Call");
    }
}

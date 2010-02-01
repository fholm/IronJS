using System.Reflection;

namespace IronJS.Compiler
{
    public class IjsFunc
    {
        public MethodInfo MethodInfo;

        public IjsFunc(MethodInfo methodInfo)
        {
            MethodInfo = methodInfo;
        }
    }

    public class IjsFunc<T> : IjsFunc
    {
        public T Closure;

        public IjsFunc(MethodInfo methodInfo, T closure)
            : base(methodInfo)
        {
            Closure = closure;
        }
    }
}

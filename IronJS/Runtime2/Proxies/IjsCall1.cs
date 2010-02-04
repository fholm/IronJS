using System;
using Microsoft.Scripting.Utils;

namespace IronJS.Runtime2.Js.Proxies
{
    public class IjsCall1<T>
    {
        public IjsFunc Proxy;
        public Func<T, bool> Guard;
        public Func<IjsClosure, T, object> Func;

        public Type GuardType { get { return Guard.GetType(); } }
        public Type FuncType { get { return Func.GetType(); } }
    }
}

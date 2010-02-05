using System;
using Microsoft.Scripting.Utils;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Runtime2.Js.Proxies
{
    public class IjsCall1<T>
    {
        public IjsFunc Func;
        public Func<T, bool> Guard;
        public Func<IjsClosure, T, object> Delegate;

        public Type GuardType { get { return Guard.GetType(); } }
        public Type DelegateType { get { return Delegate.GetType(); } }
    }
}

using System;
using System.Collections.Generic;
using System.Dynamic;
using IronJS.Compiler.Ast;
using IronJS.Tools;
using Microsoft.Scripting.Utils;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Runtime2.Js
{
    public class IjsFunc : IjsObj
    {
        public readonly Function Ast;
        public readonly IjsClosure Closure;

        public Type ClosureType { get { return Closure.GetType(); } }

        public Func<IjsClosure, object> Func0;
        public Dictionary<Type, Delegate> FuncCache;

        public IjsFunc(Function node, IjsClosure closure)
        {
            Ast = node;
            Closure = closure;
			FuncCache = new Dictionary<Type, Delegate>();
        }

        public TFunc Compile<TFunc, TGuard>(object[] values, out TGuard guard)
			where TFunc : class
			where TGuard : class
        {
            Type[] types = typeof(TFunc).GetGenericArguments();
            Type[] paramTypes = ArrayTools.DropFirstAndLast(types);

            guard = null;
            return null;
        }
    }
}

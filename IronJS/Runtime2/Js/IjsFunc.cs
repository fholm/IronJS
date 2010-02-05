using System;
using System.Collections.Generic;
using System.Dynamic;
using IronJS.Compiler.Ast;
using IronJS.Tools;

#if CLR2
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Runtime2.Js
{
    public class IjsFunc : IjsObj, IDynamicMetaObjectProvider
    {
        public readonly FuncNode Node;
        public readonly IjsClosure Closure;

        public Type ClosureType { get { return Closure.GetType(); } }

        public Func<IjsClosure, object> Func0;
        public Dictionary<Type, object> FuncCache;

        public IjsFunc(FuncNode node, IjsClosure closure)
        {
            Node = node;
            Closure = closure;
            FuncCache = new Dictionary<Type, object>();
        }

        public object Invoke0()
        {
            if (Func0 == null)
            {
                Func<bool> guard;

                Func0 = Node.Compile<Func<IjsClosure, object>, Func<bool>>(
                    Type.EmptyTypes, out guard
                );
            }
            
            return Func0(Closure);
        }

        public TFunc CreateN<TFunc, TGuard>(object[] values, out TGuard guard)
        {
            return Node.Compile<TFunc, TGuard>(
                ArrayTools.GetTypes(values), out guard
            );
        }

        #region IDynamicMetaObjectProvider Members

        public DynamicMetaObject GetMetaObject(Expression parameter)
        {
            return new Meta.IjsProxy(parameter, this);
        }

        #endregion
    }
}

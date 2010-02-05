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
        public Dictionary<Type, Delegate> FuncCache;

        public IjsFunc(FuncNode node, IjsClosure closure)
        {
            Node = node;
            Closure = closure;
			FuncCache = new Dictionary<Type, Delegate>();
        }

        public void Compile0()
        {
            if (Func0 == null)
            {
                Func<bool> guard;

                Func0 = Node.Compile<Func<IjsClosure, object>, Func<bool>>(
                    Type.EmptyTypes, out guard
                );
            }
        }

        public TFunc CompileN<TFunc, TGuard>(object[] values, out TGuard guard)
			where TFunc : class
			where TGuard : class
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

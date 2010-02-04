using System;
using System.Linq;
using System.Collections.Generic;
using System.Dynamic;
using IronJS.Compiler.Ast;
using Microsoft.Scripting.Utils;

namespace IronJS.Runtime2.Js
{
    public class IjsProxy : IjsObj, IDynamicMetaObjectProvider
    {
        public readonly FuncNode Node;
        public readonly IjsClosure Closure;
        public Type ClosureType { get { return Closure.GetType(); } }
        public Func<IjsClosure, object> Func0;
        public Dictionary<Type, object> FuncCache;

        public IjsProxy(FuncNode node, IjsClosure closure)
        {
            Node = node;
            Closure = closure;
            FuncCache = new Dictionary<Type, object>();
        }

        public object Invoke0()
        {
            if (Func0 == null)
                Func0 = (Func<IjsClosure, object>)(Node.Compile(ClosureType, Type.EmptyTypes, Type.EmptyTypes).Item2);
            
            return Func0(Closure);
        }

        public Delegate CreateN(Type delegateType, object[] values, out Delegate guard)
        {
            var types = delegateType.GetGenericArguments();

            var pair = Node.Compile(
                types[0],
                values.Select(x => x.GetType()).ToArray(),
                ArrayUtils.RemoveLast(
                    ArrayUtils.RemoveFirst(types)
                )
            );

            guard = pair.Item1;
            return pair.Item2;
        }

        #region IDynamicMetaObjectProvider Members

        public DynamicMetaObject GetMetaObject(System.Linq.Expressions.Expression parameter)
        {
            return new Meta.IjsProxy(parameter, this);
        }

        #endregion
    }
}

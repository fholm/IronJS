using System;
using System.Dynamic;
using IronJS.Compiler.Ast;

namespace IronJS.Runtime2.Js
{
    public class IjsProxy : IDynamicMetaObjectProvider
    {
        public readonly FuncNode Node;
        public readonly IjsClosure Closure;
        public Type ClosureType { get { return Closure.GetType(); } }

        public IjsProxy(FuncNode node, IjsClosure closure)
        {
            Node = node;
            Closure = closure;
        }

        #region IDynamicMetaObjectProvider Members

        public DynamicMetaObject GetMetaObject(System.Linq.Expressions.Expression parameter)
        {
            return new Meta.IjsProxy(parameter, this);
        }

        #endregion
    }
}

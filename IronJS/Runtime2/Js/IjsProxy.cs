using System;
using System.Dynamic;
using IronJS.Compiler.Ast;
using System.Collections.Generic;

namespace IronJS.Runtime2.Js
{
    public class IjsProxy : IjsObj, IDynamicMetaObjectProvider
    {
        public readonly FuncNode Node;
        public readonly IjsClosure Closure;
        public Type ClosureType { get { return Closure.GetType(); } }

        // 0
        Func<IjsClosure, object> _func0;

        // 1
        Func<object, bool> _func1Guard;
        Func<IjsClosure, object, object> _func1;
        Tuple<Func<object, bool>, Func<IjsClosure, object, object>>[] _func1Cache; 

        public IjsProxy(FuncNode node, IjsClosure closure)
        {
            Node = node;
            Closure = closure;
        }

        public object Call0()
        {
            if (_func0 == null)
                _func0 = (Func<IjsClosure, object>) (Node.Compile(ClosureType).Item2);
            
            return _func0(Closure);
        }

        public object Call1(object arg1)
        {
            if (_func1 == null)
            {
                _func1Cache = new Tuple<Func<object, bool>, Func<IjsClosure, object, object>>[10];

                var result = Node.Compile(ClosureType, arg1.GetType());

                _func1Guard = (Func<object, bool>) result.Item1;
                _func1 = (Func<IjsClosure, object, object>) result.Item2;

                _func1Cache[0] = Tuple.Create(_func1Guard, _func1);
            }

            if (_func1Guard(arg1))
                return _func1(Closure, arg1);

            throw new NotImplementedException();
        }

        #region IDynamicMetaObjectProvider Members

        public DynamicMetaObject GetMetaObject(System.Linq.Expressions.Expression parameter)
        {
            return new Meta.IjsProxy(parameter, this);
        }

        #endregion
    }
}

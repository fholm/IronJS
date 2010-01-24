using System;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Builtins
{
    public class Function_ctor : Obj, IFunction
    {
        public IFunction Function_prototype { get; private set; }

        protected Function_ctor(Context context)
        {
            Context = context;
            Function_prototype = new Function_prototype(context);
            Put("prototype", Function_prototype);
        }

        public IObj Construct()
        {
            return Construct(null);
        }

        #region IFunction Members

        public object Call(IObj that, object[] args)
        {
            throw new NotImplementedException();
        }

        public IObj Construct(object[] args)
        {
            throw new NotImplementedException();
        }

        public bool HasInstance(object obj)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region IDynamicMetaObjectProvider Members

        public Meta GetMetaObject(Et parameter)
        {
            return new IFunctionMeta(parameter, this);
        }

        #endregion

        #region Static

        static public Function_ctor Create(Context context)
        {
            return new Function_ctor(context);
        }

        #endregion 
    }
}

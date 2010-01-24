using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Js
{
    abstract class NativeFunction : Obj, IFunction
    {
        public NativeFunction(Context context)
            : this(context, context.FunctionConstructor.Function_prototype)
        {

        }

        public NativeFunction(Context context, IFunction prototype)
        {
            Context = context;
            Class = ObjClass.Function;
            Prototype = prototype;
        }

        public bool HasArgs(object[] args, int length = 1)
        {
            return args != null && args.Length >= length;
        }

        #region IFunction Members

        abstract public object Call(IObj that, object[] args);

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
    }
}

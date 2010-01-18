using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Js;

namespace IronJS.Runtime
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;

    public class Function : Obj, IFunction
    {
        public Function(Scope scope, Lambda lambda)
        {
            Scope = scope;
            Lambda = lambda;
        }

        #region IFunction Members

        public Scope Scope { get; protected set; }
        public Lambda Lambda { get; protected set; }

        public IObj Construct()
        {
            return Context.CreateObject(this);
        }

        #endregion

        #region IDynamicMetaObjectProvider Members

        public Meta GetMetaObject(Et parameter)
        {
            return null;
            //return new IFunctionMeta(parameter, this);
        }

        #endregion
    }
}

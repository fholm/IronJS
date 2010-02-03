using System;
using System.Linq.Expressions;
using IronJS.Runtime2.Js;

namespace IronJS.Compiler
{
    public class IjsParameter : IjsCloseableVar
    {
        #region IjsIVarInfo Members

        public bool IsClosedOver { get; set; }
        public ParameterExpression Expr { get; set; }

        public Type ExprType
        {
            get
            {
                return Expr == null ? IjsTypes.Undefined : Expr.Type;
            }
        }

        #endregion
    }
}

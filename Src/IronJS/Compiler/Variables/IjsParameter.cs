using System;
using IronJS.Runtime2.Js;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

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

using System;
using System.Linq;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq.Expressions;

namespace IronJS.Runtime.Js
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using EtParam = System.Linq.Expressions.ParameterExpression;

    public class Lambda
    {
        public LambdaType Delegate { get; protected set; }
        public string[] Params { get; protected set; }

        public Lambda(LambdaType func, string[] parms)
        {
            Delegate = func;
            Params = parms;
        }

        public Lambda(LambdaType func)
            : this(func, new string[] { })
        {

        }

        #region Static

        #region Expression Tree

        static internal Et EtNew(Expression<LambdaType> func, Et parms)
        {
            return AstUtils.SimpleNewHelper(
                typeof(Lambda).GetConstructor(
                    new[] { 
                        typeof(LambdaType), 
                        typeof(string[])
                    }
                ),
                func,
                parms
            );
        }

        #endregion

        #endregion
    }
}

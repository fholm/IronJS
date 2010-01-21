using System.Linq.Expressions;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Runtime.Js
{
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

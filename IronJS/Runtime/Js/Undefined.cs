using System.Linq.Expressions;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Runtime.Js
{
    public sealed class Undefined
    {
        static Undefined _instance;
        static readonly object _sync = new object();

        public static Undefined Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_sync)
                    {
                        if(_instance == null)
                            _instance = new Undefined();
                    }
                }

                return _instance;
            }
        }

        internal static ConstantExpression Expr
        {
            get
            {
                return Et.Constant(Instance);
            }
        }

        private Undefined()
        {

        }

        public override string ToString()
        {
            return "undefined";
        }
    }
}

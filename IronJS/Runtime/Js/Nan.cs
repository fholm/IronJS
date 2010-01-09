using System.Linq.Expressions;

namespace IronJS.Runtime.Js
{
    using Et = System.Linq.Expressions.Expression;

    public sealed class Nan
    {
        static Nan _instance;
        static readonly object _sync = new object();

        public static Nan Instance
        {
            get
            {
                if (_instance == null)
                {
                    lock (_sync)
                    {
                        if (_instance == null)
                            _instance = new Nan();
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

        private Nan()
        {

        }

        public override string ToString()
        {
            return "NaN";
        }
    }
}

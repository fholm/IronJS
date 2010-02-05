
#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Runtime2.Js
{
    using Et = Expression;

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

        internal static Et StaticExpr
        {
            get
            {
                return Et.Property(
                    null,
                    typeof(Undefined).GetProperty("Instance")
                );
            }
        }

        private Undefined()
        {

        }
    }
}

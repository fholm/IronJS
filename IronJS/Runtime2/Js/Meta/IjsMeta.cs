using System.Dynamic;
using Et = System.Linq.Expressions.Expression;
using MetaObj = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime2.Js.Meta
{
    public class IjsMeta<T> : MetaObj where T : class
    {
        protected T Self { get { return (T)Value; } }
        protected Et SelfExpr { get { return Et.Convert(Expression, typeof(T)); } }

        public IjsMeta(Et expr, T value)
            : base(expr, BindingRestrictions.Empty, value)
        {

        }
    }
}

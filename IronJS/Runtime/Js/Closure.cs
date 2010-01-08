
namespace IronJS.Runtime.Js
{
    using System.Dynamic;
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;

    class Closure : IDynamicMetaObjectProvider
    {
        public readonly Frame<Function> Table;
        public readonly Frame<object> Frame;
        public readonly Function Function;

        public Closure(Frame<Function> table, Frame<object> frame, Function function)
        {
            Table = table;
            Frame = frame;
            Function = function;
        }

        #region IDynamicMetaObjectProvider Members

        Meta IDynamicMetaObjectProvider.GetMetaObject(Et parameter)
        {
            return new ClosureMeta(parameter, this);
        }

        #endregion
    }
}

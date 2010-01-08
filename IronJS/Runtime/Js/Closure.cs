using System.Dynamic;

namespace IronJS.Runtime.Js
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;

    class Closure : IDynamicMetaObjectProvider
    {
        public readonly Frame Frame;
        public readonly Function Function;

        public Closure(Frame frame, Function function)
        {
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

using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Js
{
    abstract public class NativeFunction : Obj, IFunction
    {
        public NativeFunction(Context context)
            : this(context, context.FunctionConstructor.Function_prototype)
        {

        }

        public NativeFunction(Context context, IObj prototype)
        {
            Context = context;
            Class = ObjClass.Function;
            Prototype = prototype;
        }

        public bool HasArgs(object[] args, int length = 1)
        {
            return args != null && args.Length >= length;
        }

        #region IFunction Members

        public abstract object Call(IObj that, object[] args);

        #endregion

        #region IDynamicMetaObjectProvider Members

        public override Meta GetMetaObject(Et parameter)
        {
            return new IFunctionMeta(parameter, this);
        }

        #endregion
    }
}

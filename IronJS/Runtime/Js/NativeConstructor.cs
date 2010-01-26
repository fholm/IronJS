using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Js
{
    abstract public class NativeConstructor : NativeFunction, IConstructor
    {
        public NativeConstructor(Context context)
            : base(context)
        {

        }

        public NativeConstructor(Context context, IObj prototype)
            : base(context, prototype)
        {

        }

        public override Meta GetMetaObject(Et parameter)
        {
            return new IConstructorMeta(parameter, this);
        }

        #region IConstructor Members

        abstract public IObj Construct(object[] args);

        public bool HasInstance(object obj)
        {
            if (!(obj is IObj))
                return false;

            var prototype = this.Get("prototype");

            if (!(prototype is IObj))
                throw InternalRuntimeError.New("prototype property is not a object");

            var jsObj = (IObj)obj;

            if (jsObj.Prototype == null)
                return false;

            return jsObj.Prototype == prototype;
        }

        #endregion

        public override string ToString()
        {
            return "IronJS: NativeConstructor";
        }
    }
}

using IronJS.Runtime.Js.Descriptors;
using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;

namespace IronJS.Runtime.Js
{
    public class UserFunction : Obj, IConstructor
    {
        public Scope Scope { get; protected set; }
        public Lambda Lambda { get; protected set; }

        public UserFunction(Scope scope, Lambda lambda)
        {
            Scope = scope;
            Lambda = lambda;
            Set("length",
                new UserProperty(
                    this,
                    (double) lambda.Params.Length
                )
            );
        }

        #region IFunction Members

        public object Call(IObj that, object[] args)
        {
            var callScope = Scope.CreateCallScope(Scope, this, that, args, Lambda.Params);
            return Lambda.Delegate.Invoke(callScope);
        }

        #endregion

        #region IConstructor Members

        public IObj Construct(object[] args)
        {
            var newObject = Context.ObjectConstructor.Construct();
            var callScope = Scope.CreateCallScope(Scope, this, newObject, args, Lambda.Params);

            var prototype = this.Get("prototype");

            (newObject as Obj).Prototype = (prototype is IObj)
                                           ? (prototype as IObj)
                                           : Context.ObjectConstructor.Object_prototype;

            Lambda.Delegate.Invoke(callScope);
            return newObject;
        }

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

        #region IDynamicMetaObjectProvider Members

        public override Meta GetMetaObject(Et parameter)
        {
            return new IConstructorMeta(parameter, this);
        }

        #endregion
    }
}

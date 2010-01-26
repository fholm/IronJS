using IronJS.Runtime.Js;
using IronJS.Runtime.Js.Descriptors;

namespace IronJS.Runtime.Builtins
{
    class Object_prototype_isPrototypeOf : NativeFunction
    {
        public Object_prototype_isPrototypeOf(Context context)
            : base(context)
        {
            Set("length", 
                new UserProperty(this, 1.0D)
            );
        }

        public override object Call(IObj that, object[] args)
        {
            if (HasArgs(args) && args[0] is IObj)
                return (args[0] as IObj).Prototype == that;

            return false;
        }
    }
}

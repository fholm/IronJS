using IronJS.Runtime.Js;
using IronJS.Runtime.Js.Descriptors;

namespace IronJS.Runtime.Builtins
{
    class Object_prototype_hasOwnProperty : NativeFunction
    {
        public Object_prototype_hasOwnProperty(Context context)
            : base(context)
        {
            Set("length",
                new UserProperty(this, 1.0D)
            );
        }

        public override object Call(IObj that, object[] args)
        {
            if (HasArgs(args))
                return that.Has(args[0]);

            return false;
        }
    }
}

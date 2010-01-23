using IronJS.Runtime.Js;

namespace IronJS.Runtime.Builtins
{
    class Object_prototype_hasOwnProperty : NativeFunction
    {
        public Object_prototype_hasOwnProperty(Context context)
            : base(context)
        {
            SetOwnProperty("length", 1);
        }

        public override object Call(IObj that, object[] args)
        {
            if (HasArgs(args))
                return that.HasOwnProperty(args[0]);

            return false;
        }
    }
}

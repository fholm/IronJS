using IronJS.Runtime.Js;

namespace IronJS.Runtime.Builtins
{
    class Object_prototype_toLocaleString : NativeFunction
    {
        public Object_prototype_toLocaleString(Context context)
            : base(context)
        {
            SetOwnProperty("length", 0);
        }

        public override object Call(IObj that, object[] args)
        {
            return (that.Get("toString") as IFunction).Call(that, args);
        }
    }
}

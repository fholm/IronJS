using IronJS.Runtime.Js;

namespace IronJS.Runtime.Builtins
{
    class Object_prototype_valueOf : NativeFunction
    {
        public Object_prototype_valueOf(Context context)
            : base(context)
        {
            SetOwnProperty("length", 0);
        }

        public override object Call(IObj that, object[] args)
        {
            return that;
        }
    }
}

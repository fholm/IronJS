using IronJS.Runtime.Js;
using IronJS.Runtime.Js.Descriptors;

namespace IronJS.Runtime.Builtins
{
    class Object_prototype_toString : NativeFunction
    {
        public Object_prototype_toString(Context context)
            : base(context)
        {
            this.Set("length", 0);
        }

        public override object Call(IObj that, object[] args)
        {
            return "[object " + that.Class + "]";
        }
    }
}

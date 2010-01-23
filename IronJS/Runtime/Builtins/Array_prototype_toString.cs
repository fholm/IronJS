using IronJS.Runtime.Js;

namespace IronJS.Runtime.Builtins
{
    class Array_prototype_toString : NativeFunction
    {
        public Array_prototype_toString(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            if (!(that is ArrayObj))
                throw new ShouldThrowTypeError();

            return ((that as IObj).Get("join") as IFunction).Call(that, new object[] { });
        }
    }
}

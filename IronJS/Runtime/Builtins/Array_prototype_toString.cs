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
            if (!(that is JsArray))
                throw new ShouldThrowTypeError();

            return ((that as IObj).Search("join") as IFunction).Call(that, new object[] { });
        }
    }
}

using IronJS.Runtime.Js;

namespace IronJS.Runtime.Builtins
{
    class String_prototype_toString : NativeFunction
    {
        public String_prototype_toString(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            if (that.Class != ObjClass.String || !that.HasValue())
                throw new ShouldThrowTypeError();

            return (that as IValueObj).Value;
        }
    }
}

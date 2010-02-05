using IronJS.Runtime.Js;

namespace IronJS.Runtime.Builtins
{
    class Boolean_prototype_valueOf: NativeFunction
    {
        public Boolean_prototype_valueOf(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            if (that.Class != ObjClass.Boolean || !that.HasValue())
                throw new ShouldThrowTypeError();

            return (that as ValueObj).Value;
        }

    }
}

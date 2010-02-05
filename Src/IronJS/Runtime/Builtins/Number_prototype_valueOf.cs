using IronJS.Runtime.Js;

namespace IronJS.Runtime.Builtins
{
    class Number_prototype_valueOf : NativeFunction
    {
        public Number_prototype_valueOf(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            if (that.Class != ObjClass.Number || !that.HasValue())
                throw new ShouldThrowTypeError();

            return (that as ValueObj).Value;
        }

    }
}

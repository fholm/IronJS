using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class Number_prototype_toLocaleString : NativeFunction
    {
        public Number_prototype_toLocaleString(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            if (that.Class != ObjClass.Number || !that.HasValue())
                throw new ShouldThrowTypeError();

            return JsTypeConverter.ToString((that as ValueObj).Value);
        }

    }
}

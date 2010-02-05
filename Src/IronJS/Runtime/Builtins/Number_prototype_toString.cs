using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class Number_prototype_toString : NativeFunction
    {
        public Number_prototype_toString(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            if (that.Class != ObjClass.Number || !that.HasValue())
                throw new ShouldThrowTypeError();

            //TODO: this should handle args[0] (radix) correctly.
            return JsTypeConverter.ToString((that as ValueObj).Value);
        }

    }
}

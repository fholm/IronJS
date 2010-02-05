using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class Boolean_prototype_toString : NativeFunction
    {
        public Boolean_prototype_toString(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            if (that.Class != ObjClass.Boolean || !that.HasValue())
                throw new ShouldThrowTypeError();

            return JsTypeConverter.ToBoolean((that as ValueObj).Value) ? "true" : "false";
        }

    }
}

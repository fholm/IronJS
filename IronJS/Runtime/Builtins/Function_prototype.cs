using IronJS.Runtime.Js;
using IronJS.Runtime.Js.Descriptors;

namespace IronJS.Runtime.Builtins
{
    class Function_prototype : NativeFunction
    {
        public Function_prototype(Context context)
            : base(context, null)
        {
            Set("toString", 
                new UserProperty(
                    this, new Function_prototype_toString(context, this)
                )
            );

            Set("apply", 
                new UserProperty(
                    this, new Function_prototype_apply(context, this)
                )
            );

            Set("call", 
                new UserProperty(
                    this, new Function_prototype_call(context, this)
                )
            );
        }

        public override object Call(Js.IObj that, object[] args)
        {
            return Js.Undefined.Instance;
        }
    }
}

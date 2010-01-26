using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Js.Descriptors;

namespace IronJS.Runtime.Builtins
{
    class Function_prototype_toString: NativeFunction
    {
        public Function_prototype_toString(Context context, IObj function_prototype)
            : base(context, function_prototype)
        {
            Set("length",
                new UserProperty(this, 0.0D)
            );
        }

        public override object Call(IObj that, object[] args)
        {
            throw new NotImplementedException();
        }
    }
}

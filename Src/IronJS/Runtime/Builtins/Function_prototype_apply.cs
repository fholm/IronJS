using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Js.Descriptors;

namespace IronJS.Runtime.Builtins
{
    class Function_prototype_apply : NativeFunction
    {
        public Function_prototype_apply(Context context, IFunction function_prototype)
            : base(context, function_prototype)
        {
            Set("length",
                new UserProperty(this, 2.0D)
            );
        }

        public override object Call(IObj that, object[] args)
        {
            throw new NotImplementedException();
        }
    }
}

using System;
using IronJS.Runtime.Js;

namespace IronJS.Runtime.Builtins
{
    class Function_prototype_apply : NativeFunction
    {
        public Function_prototype_apply(Context context, IFunction function_prototype)
            : base(context, function_prototype)
        {
            SetOwnProperty("length", 2);
        }

        public override object Call(IObj that, object[] args)
        {
            throw new NotImplementedException();
        }
    }
}

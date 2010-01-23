using System;
using IronJS.Runtime.Js;

namespace IronJS.Runtime.Builtins
{
    class Function_prototype_toString: NativeFunction
    {
        public Function_prototype_toString(Context context, IFunction function_prototype)
            : base(context, function_prototype)
        {
            SetOwnProperty("length", 0);
        }

        public override object Call(IObj that, object[] args)
        {
            throw new NotImplementedException();
        }
    }
}

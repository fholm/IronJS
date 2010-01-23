using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Microsoft.Scripting.Utils;

namespace IronJS.Runtime.Builtins
{
    class Function_prototype_call : NativeFunction
    {
        public Function_prototype_call(Context context, IFunction function_prototype)
            : base(context, function_prototype)
        {
            SetOwnProperty("length", 1);
        }

        public override object Call(IObj that, object[] args)
        {
            if (!(that is IFunction))
                throw new ShouldThrowTypeError();

            IObj callThat = null;

            if (HasArgs(args))
            {
                if (args[0] != null && !(args[0] is Undefined))
                    callThat = JsTypeConverter.ToObject(args[0], Context);
            }
            else
            {
                throw new NotImplementedException();
            }

            return (that as IFunction).Call(callThat, ArrayUtils.RemoveFirst(args));
        }
    }
}

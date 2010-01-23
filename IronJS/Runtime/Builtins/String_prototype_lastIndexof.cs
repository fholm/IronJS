using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class String_prototype_lastIndexOf : NativeFunction
    {
        public String_prototype_lastIndexOf(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            if (!HasArgs(args))
                throw new ArgumentException();

            var target = JsTypeConverter.ToString(that);
            var search = JsTypeConverter.ToString(args[0]);
            var index = JsTypeConverter.ToInt32(args.Length > 1 ? args[1] : int.MaxValue);

            return (double)target.LastIndexOf(search, index == int.MaxValue ? target.Length - 1 : index);
        }

    }
}

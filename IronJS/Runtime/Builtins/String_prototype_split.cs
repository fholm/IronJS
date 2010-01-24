using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class String_prototype_split : NativeFunction
    {
        public String_prototype_split(Context context)
            : base(context)
        {
            SetOwnProperty("length", 2.0D);
        }

        public override object Call(IObj that, object[] args)
        {
            var str = JsTypeConverter.ToString(that);
            var results = Context.ArrayConstructor.Construct();

            if (!HasArgs(args))
            {
                results.Put(0.0D, str);
                return results;
            }

            var arg0 = args[0];
            var limit = args.Length > 1 ? JsTypeConverter.ToInt32(args[1]) : int.MaxValue;
            string split = "";

            if ((arg0 is IObj) && (arg0 as IObj).Class == ObjClass.RegExp)
            {
                throw new NotImplementedException();
            }
            else
            {
                split = JsTypeConverter.ToString(arg0);

                var parts = str.Split(new[] { split }, limit, StringSplitOptions.None);
                var k = 0.0D;

                foreach (var part in parts)
                    results.Put(k++, part);
            }

            return results;
        }
    }
}

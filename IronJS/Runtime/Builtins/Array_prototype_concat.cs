using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Microsoft.Scripting.Utils;

namespace IronJS.Runtime.Builtins
{
    class Array_prototype_concat : NativeFunction
    {
        public Array_prototype_concat(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {
            var arrayObj = Context.ArrayConstructor.Construct();
            var k = 0.0D;

            args = ArrayUtils.Insert((object)that, args);

            foreach (var arg in args)
            {
                if (arg is ArrayObj)
                {
                    var arr = arg as ArrayObj;
                    var len = (double)arr.Length;
                    for (var i = 0.0D; i < len; ++i)
                    {
                        if (arr.HasOwnProperty(i))
                            arrayObj.SetOwnProperty(k++, arr.Get(i));
                    }
                }
                else
                {
                    arrayObj.SetOwnProperty(k++, JsTypeConverter.ToString(arg));
                }
            }

            return arrayObj;
        }
    }
}

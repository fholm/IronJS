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
                if (arg is JsArray)
                {
                    var arr = arg as JsArray;
                    var len = (double)arr.Length;
                    for (var i = 0.0D; i < len; ++i)
                    {
                        if (arr.HasOwn(i))
                            arrayObj.SetOwn(k++, arr.Get(i));
                    }
                }
                else
                {
                    arrayObj.SetOwn(k++, JsTypeConverter.ToString(arg));
                }
            }

            return arrayObj;
        }
    }
}

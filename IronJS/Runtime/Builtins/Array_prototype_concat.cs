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
            var k = 0;
            var objs = ArrayUtils.Insert((object)that, args);
            var arrayObj = Context.ArrayConstructor.Construct();

            foreach (var arg in objs)
            {
                if (arg is JsArray)
                {
                    var arr = arg as JsArray;
                    var len = arr.Length;

                    for (var i = 0; i < len; ++i)
                    {
                        if (arr.Has(i))
                        {
                            arrayObj.Set(k++, arr.Get(i));
                        }
                    }
                }
                else
                {
                    arrayObj.Set(k++, JsTypeConverter.ToString(arg));
                }
            }

            return arrayObj;
        }
    }
}

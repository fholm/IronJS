using System;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Builtins
{
    class Array_prototype_reverse : NativeFunction
    {
        public Array_prototype_reverse(Context context)
            : base(context)
        {

        }

        public override object Call(IObj that, object[] args)
        {

            var len = JsTypeConverter.ToNumber(
                     that.Get("length")
                 );

            var half = Math.Floor(len / 2);

            var begin = 0.0D;
            var end = 0.0D;

            object beginValue;
            object endValue;

            bool hasBegin;
            bool hasEnd;

            while (true)
            {
                if (begin == half)
                    return that;

                end = len - begin - 1;

                hasBegin = that.TryGet(begin, out beginValue);
                hasEnd = that.TryGet(end, out endValue);

                if (!hasEnd)
                {
                    if (!hasBegin)
                    {
                        that.TryDelete(begin);
                        that.TryDelete(end);
                    }
                    else
                    {
                        that.SetOwn(end, beginValue);
                        that.TryDelete(begin);
                    }
                }
                else if (!hasBegin)
                {
                    if (!hasEnd)
                    {
                        that.TryDelete(begin);
                        that.TryDelete(end);
                    }
                    else
                    {
                        that.SetOwn(begin, endValue);
                        that.TryDelete(end);
                    }
                }
                else
                {
                    that.SetOwn(begin, endValue);
                    that.SetOwn(end, beginValue);
                }

                ++begin;
            }
        }
    }
}

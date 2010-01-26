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

            IDescriptor<IObj> beginDescriptor;
            IDescriptor<IObj> endDescriptor;

            bool hasBegin;
            bool hasEnd;

            while (true)
            {
                if (begin == half)
                    return that;

                end = len - begin - 1;

                hasBegin = that.Get(begin, out beginDescriptor);
                hasEnd = that.Get(end, out endDescriptor);

                if (!hasEnd)
                {
                    if (!hasBegin)
                    {
                        that.TryDelete(begin);
                        that.TryDelete(end);
                    }
                    else
                    {
                        that.Set(end, beginDescriptor.Get());
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
                        that.Set(begin, endDescriptor.Get());
                        that.TryDelete(end);
                    }
                }
                else
                {
                    that.Set(begin, endDescriptor.Get());
                    that.Set(end, beginDescriptor.Get());
                }

                ++begin;
            }
        }
    }
}

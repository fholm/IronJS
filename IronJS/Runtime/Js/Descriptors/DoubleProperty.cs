using System;
using IronJS.Runtime.Utils;
namespace IronJS.Runtime.Js.Descriptors
{
    public class DoubleProperty : NativeProperty
    {
        public DoubleProperty(IObj owner, bool isReadOnly = false, bool isDeletable = true, bool isEnumerable = true)
            : base(owner, isReadOnly, isDeletable, isEnumerable)
        {

        }

        public object Set(object value)
        {
            Value = (value is IObj)
                    ? JsTypeConverter.ToNumber(value)
                    : Convert.ToDouble(value);

            return Value;
        }
    }
}

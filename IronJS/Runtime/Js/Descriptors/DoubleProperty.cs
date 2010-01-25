using System;
using IronJS.Runtime.Utils;
namespace IronJS.Runtime.Js.Descriptors
{
    public class DoubleProperty : NativeProperty
    {
        public DoubleProperty(IObj owner, object value = null, bool isReadOnly = false, bool isDeletable = true, bool isEnumerable = true)
            : base(owner, value, isReadOnly, isDeletable, isEnumerable)
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

using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Js.Descriptors
{
    public class LengthProperty : IDescriptor<JsArray>
    {
        public int Value { get; protected set; }

        public LengthProperty(JsArray owner)
        {
            Owner = owner;
            Value = 0;
        }

        public void Update(int value)
        {
            if (Value <= value)
                Value = value + 1;
        }

        #region IDescriptor<JsArray> Members

        public JsArray Owner { get; protected set; }
        public bool IsEnumerable { get { return false; } }
        public bool IsReadOnly { get { return false; } }
        public bool IsDeletable { get { return false; } }

        public object Get()
        {
            return (double)Value;
        }

        public object Set(object value)
        {
            var newValue = JsTypeConverter.ToInt32(value);

            var n = Value - 1;

            for (; n >= newValue; --n)
                Owner.TryDelete(n);

            return Value = newValue;
        }

        #endregion
    }
}

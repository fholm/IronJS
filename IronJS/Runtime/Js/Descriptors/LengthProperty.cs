using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Js.Descriptors
{
    public class LengthProperty : IDescriptor<JsArray>
    {
        public LengthProperty(JsArray owner)
        {
            Owner = owner;
        }

        #region IDescriptor<JsArray> Members

        public JsArray Owner { get; protected set; }
        public bool IsEnumerable { get { return false; } }
        public bool IsReadOnly { get { return false; } }
        public bool IsDeletable { get { return false; } }

        public object Get()
        {
            return (double) Owner.Values.Length;
        }

        public object Set(object value)
        {
            return null;
        }

        #endregion
    }
}

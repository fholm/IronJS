namespace IronJS.Runtime.Js.Descriptors
{
    public class UserProperty : IDescriptor<IObj>
    {
        protected object Value;

        public UserProperty(IObj owner, object value = null)
        {
            Owner = owner;
            Value = value ?? Undefined.Instance;
        }

        #region IDescriptor<IObj> Members

        public IObj Owner { get; protected set; }
        public bool IsEnumerable { get { return true; } }
        public bool IsReadOnly { get { return false; } }
        public bool IsDeletable { get { return true; } }

        public object Get()
        {
            return Value;
        }

        public object Set(object value)
        {
            return Value = value;
        }

        #endregion
    }
}

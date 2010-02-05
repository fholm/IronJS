namespace IronJS.Runtime.Js.Descriptors
{
    public class NativeProperty : IDescriptor<IObj>
    {
        protected object Value;

        public NativeProperty(IObj owner, object value, 
                              bool isReadOnly, bool isDeletable, 
                              bool isEnumerable)
        {
            Owner = owner;
            Value = value ?? Undefined.Instance;

            IsReadOnly = isReadOnly;
            IsDeletable = isDeletable;
            IsEnumerable = isEnumerable;
        }

        #region IDescriptor<IObj> Members

        public IObj Owner { get; protected set; }
        public bool IsEnumerable { get; protected set; }
        public bool IsReadOnly { get; protected set; }
        public bool IsDeletable { get; protected set; }

        public virtual object Get()
        {
            return Value;
        }

        public virtual object Set(object value)
        {
            if (IsReadOnly)
                throw InternalRuntimeError.New(
                    InternalRuntimeError.PROPERTY_READONLY
                );

            return Value = value;
        }

        #endregion
    }
}

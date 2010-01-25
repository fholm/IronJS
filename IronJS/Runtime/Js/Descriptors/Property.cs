using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Runtime.Js.Descriptors
{
    public class Property : IDescriptor<IObj>
    {
        protected object Value;

        public Property(IObj owner, bool isEnumerable = true, bool isDeletable = true, bool isReadOnly = false)
        {
            Owner = owner;

            IsEnumerable = isEnumerable;
            IsDeletable = isDeletable;
            IsReadOnly = isReadOnly;
        }

        #region IDescriptor<IObj> Members

        public IObj Owner { get; protected set; }
        public bool IsEnumerable { get; protected set; }
        public bool IsReadOnly { get; protected set; }
        public bool IsDeletable { get; protected set; }

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

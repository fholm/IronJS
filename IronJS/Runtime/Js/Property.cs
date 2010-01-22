using System;

namespace IronJS.Runtime.Js
{
    [Flags]
    public enum PropertyAttrs
    { 
        ReadOnly = 1, 
        DontEnum = 2, 
        DontDelete = 4
    }

    public class Property
    {
        protected object _value;
        protected PropertyAttrs _attrs;

        public object Value
        {
            get
            {
                return _value;
            }
            set
            {
                if (NotHasAttr(PropertyAttrs.ReadOnly))
                {
                    _value = value;
                }
                else
                {
                    throw InternalRuntimeError.New(
                        InternalRuntimeError.PROPERTY_READONLY
                    );
                }
            }
        }

        public Property(object value)
            : this(value, 0)
        {

        }

        public Property(object value, PropertyAttrs attrs)
        {
            _value = value; // optimization, we can skip going through 'Value'-property here
            _attrs = attrs;
        }

        public bool HasAttr(PropertyAttrs attr)
        {
            return _attrs.HasFlag(attr);
        }

        public bool NotHasAttr(PropertyAttrs attr)
        {
            return !_attrs.HasFlag(attr);
        }
    }
}

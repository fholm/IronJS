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

    sealed class Property
    {
        object _value;

        internal PropertyAttrs Attributes;

        internal object Value
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
                    throw new RuntimeError("Property is read-only");
                }
            }
        }

        internal Property(object value)
            : this(value, 0)
        {

        }

        internal Property(object value, PropertyAttrs attrs)
        {
            _value = value; // optimization, we can skip going through 'Value'-property here
            Attributes = attrs;
        }

        internal bool HasAttr(PropertyAttrs attr)
        {
            return Attributes.HasFlag(attr);
        }

        internal bool NotHasAttr(PropertyAttrs attr)
        {
            return !HasAttr(attr);
        }
    }
}

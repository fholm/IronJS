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
        internal readonly string Name;

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
                    throw new RuntimeError("Property '{0}' is read-only", Name);
                }
            }
        }

        internal Property(string name, object value)
            : this(name, value, 0)
        {

        }

        internal Property(string name, object value, PropertyAttrs attrs)
        {
            Name = name;
            Value = value;
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

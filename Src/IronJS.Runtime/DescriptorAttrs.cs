using System;

namespace IronJS.Runtime
{
    [Flags]
    public enum DescriptorAttrs : ushort
    {
        None = 0,
        ReadOnly = 1,
        DontEnum = 2,
        DontDelete = 4,
        DontEnumOrDelete = DontEnum | DontDelete,
        Immutable = ReadOnly | DontEnum | DontDelete,
    }
}

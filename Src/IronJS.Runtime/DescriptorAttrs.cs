using System;

namespace IronJS.Runtime
{
    public static class DescriptorAttrs
    {
        public const ushort None = 0;
        public const ushort ReadOnly = 1;
        public const ushort DontEnum = 2;
        public const ushort DontDelete = 4;
        public const ushort DontEnumOrDelete = DontEnum | DontDelete;
        public const ushort Immutable = ReadOnly | DontEnum | DontDelete;
    }
}

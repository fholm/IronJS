using System;

namespace IronJS.Runtime
{
    public struct Descriptor
    {
        public BoxedValue Value;
        public ushort Attributes;
        public bool HasValue;

        public bool IsWritable { get { return (this.Attributes & DescriptorAttrs.ReadOnly) == DescriptorAttrs.None; } }
        public bool IsDeletable { get { return (this.Attributes & DescriptorAttrs.DontDelete) == DescriptorAttrs.None; } }
        public bool IsEnumerable { get { return (this.Attributes & DescriptorAttrs.DontEnum) == DescriptorAttrs.None; } }
    }
}

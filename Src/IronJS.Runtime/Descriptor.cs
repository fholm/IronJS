using System;

namespace IronJS.Runtime
{
    public static class DescriptorAttrs
    {
        public const ushort None = 0;
        public const ushort ReadOnly = 1;
        public const ushort DontEnum = 2;
        public const ushort DontDelete = 4;
        public const ushort IsAccessor = 8;
        public const ushort DontEnumOrDelete = DontEnum | DontDelete;
        public const ushort Immutable = ReadOnly | DontEnum | DontDelete;
    }

    public struct Descriptor
    {
        public BoxedValue Value;
        public ushort Attributes;
        public bool HasValue;

        public bool IsWritable 
        { get { return DescriptorUtils.IsWritable(ref this); } }

        public bool IsDeletable
        { get { return DescriptorUtils.IsDeletable(ref this); } }

        public bool IsEnumerable
        { get { return DescriptorUtils.IsEnumerable(ref this); } }

        public bool IsAccessor
        { get { return DescriptorUtils.IsAccessor(ref this); } }
    }

    public static class DescriptorUtils
    {
        public static bool IsWritable(ref Descriptor d)
        {
            return (d.Attributes & DescriptorAttrs.ReadOnly) == 0;
        }

        public static bool IsDeletable(ref Descriptor d)
        {
            return (d.Attributes & DescriptorAttrs.DontDelete) == 0;
        }

        public static bool IsEnumerable(ref Descriptor d)
        {
            return (d.Attributes & DescriptorAttrs.DontEnum) == 0;
        }

        public static bool IsAccessor(ref Descriptor d)
        {
            return (d.Attributes & DescriptorAttrs.IsAccessor) > 0;
        }
    }

    public class AccessorDescriptor
    {
        public FunctionObject Get;
        public FunctionObject Set;
    }
}

using System;

namespace IronJS.Runtime
{
    public static class DescriptorAttrs
    {
        /// <summary>
        /// Special attribute used to clear all flags
        /// </summary>
        public const ushort None = 0;

        /// <summary>
        /// Set this flag to make a data descriptor read only
        /// </summary>
        public const ushort NotWritable = 1;

        /// <summary>
        /// Set this flag to make a descriptor non enumerable
        /// </summary>
        public const ushort NotEnumerable = 2;

        /// <summary>
        /// Set this flag to make a descriptor non configurable
        /// </summary>
        public const ushort NotConfigurable = 4;

        /// <summary>
        /// Set this flag to signal that a descriptor is an accessor descriptor
        /// and not a data descriptor
        /// </summary>
        public const ushort IsAccessor = 8;

        /// <summary>
        /// Shorthand bitmask that combines NotEnumerable and NotConfigurable
        /// </summary>
        public const ushort NotEC = 
            NotEnumerable | NotConfigurable;

        /// <summary>
        /// Shorthand bitmask that combines NotWritable, NotEnumerable
        /// and NotConfigurable
        /// </summary>
        public const ushort NotWEC = 
            NotWritable | NotEnumerable | NotConfigurable;
    }

    public class AccessorDescriptor
    {
        public FunctionObject Get;
        public FunctionObject Set;
    }

    public struct Descriptor
    {
        public BoxedValue Value;
        public ushort Attributes;
        public bool HasValue;

        public bool IsWritable 
        { get { return DescriptorUtils.IsWritable(ref this); } }

        public bool IsDeletable
        { get { return DescriptorUtils.IsConfigurable(ref this); } }

        public bool IsEnumerable
        { get { return DescriptorUtils.IsEnumerable(ref this); } }
    }

    public static class DescriptorUtils
    {
        public static bool IsWritable(ref Descriptor d)
        {
            return (d.Attributes & DescriptorAttrs.NotWritable) == 0;
        }

        public static bool IsConfigurable(ref Descriptor d)
        {
            return (d.Attributes & DescriptorAttrs.NotConfigurable) == 0;
        }

        public static bool IsEnumerable(ref Descriptor d)
        {
            return (d.Attributes & DescriptorAttrs.NotEnumerable) == 0;
        }

        public static bool IsAccessor(ref Descriptor d)
        {
            return (d.Attributes & DescriptorAttrs.IsAccessor) > 0;
        }

        public static void SetNotWritable(ref Descriptor d)
        {
            d.Attributes |= DescriptorAttrs.NotWritable;
        }

        public static void SetNotEnumerable(ref Descriptor d)
        {
            d.Attributes |= DescriptorAttrs.NotEnumerable;
        }

        public static void SetNotConfigurable(ref Descriptor d)
        {
            d.Attributes |= DescriptorAttrs.NotConfigurable;
        }

        public static void SetIsAccessor(ref Descriptor d)
        {
            d.Attributes |= DescriptorAttrs.IsAccessor;
        }
    }
}

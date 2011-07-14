using System;
using System.Runtime.InteropServices;

namespace IronJS.Runtime
{
    internal static class BoxedValueOffsets
    {
        public const int ValueType = 0;
        public const int Tag = 4;
        public const int Marker = 6;
        public const int ReferenceType = 8;
    }

    public static class Markers
    {
        public const ushort Number = 0xFFF8;
        public const ushort Tagged = 0xFFF9;
    }

    /// <summary>
    /// This is a NaN-tagged struct that is used for representing
    /// values that don't have a known type at runtime
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct BoxedValue
    {
        // Reference Types
        [FieldOffset(BoxedValueOffsets.ReferenceType)]
        public object Clr;
        [FieldOffset(BoxedValueOffsets.ReferenceType)]
        public CommonObject Object;
        [FieldOffset(BoxedValueOffsets.ReferenceType)]
        public ArrayObject Array;
        [FieldOffset(BoxedValueOffsets.ReferenceType)]
        public FunctionObject Func;
        [FieldOffset(BoxedValueOffsets.ReferenceType)]
        public string String;
        [FieldOffset(BoxedValueOffsets.ReferenceType)]
        public SuffixString SuffixString;
        [FieldOffset(BoxedValueOffsets.ReferenceType)]
        public BoxedValue[] Scope;
        [FieldOffset(BoxedValueOffsets.ReferenceType)]
        public Accessor Accessor;

        // Value Types
        [FieldOffset(BoxedValueOffsets.ValueType)]
        public bool Bool;
        [FieldOffset(BoxedValueOffsets.ValueType)]
        public double Number;

        // Type & Tag
        [FieldOffset(BoxedValueOffsets.Tag)]
        public uint Tag;
        [FieldOffset(BoxedValueOffsets.Marker)]
        public ushort Marker;

        public bool IsNumber { get { return this.Marker < Markers.Tagged; } }
        public bool IsTagged { get { return this.Marker > Markers.Number; } }
        public bool IsString { get { return this.IsTagged && (this.Tag == TypeTags.String || this.Tag == TypeTags.SuffixString); } }
        public bool IsObject { get { return this.IsTagged && this.Tag >= TypeTags.Object; } }
        public bool IsFunction { get { return this.IsTagged && this.Tag >= TypeTags.Function; } }
        public bool IsBoolean { get { return this.IsTagged && this.Tag == TypeTags.Bool; } }
        public bool IsUndefined { get { return this.IsTagged && this.Tag == TypeTags.Undefined; } }
        public bool IsClr { get { return this.IsTagged && this.Tag == TypeTags.Clr; } }
        public bool IsRegExp { get { return this.IsObject && this.Object is RegExpObject; } }
        public bool IsNull { get { return this.IsClr && this.Clr == null; } }

        // As per ECMA-262, Section 8.6.2, the following types are primitive:
        //  Undefined, Null, Boolean, String, or Number
        public bool IsPrimitive
        {
            get
            {
                return this.IsUndefined || this.IsNull || this.IsBoolean || this.IsString || this.IsNumber;
            }
        }

        public object ClrBoxed
        {
            get
            {
                if (this.IsNumber)
                    return this.Number;
                if (this.Tag == TypeTags.Bool)
                    return this.Bool;
                if (this.Tag == TypeTags.SuffixString)
                    return this.SuffixString.ToString();
                return this.Clr;
            }
        }

        public T Unbox<T>()
        {
            return (T)this.ClrBoxed;
        }

        public object UnboxObject()
        {
            if (this.IsNumber)
            {
                return this.Number;
            }
            else
            {
                switch (this.Tag)
                {
                    case TypeTags.Bool:
                        return this.Bool;
                    case TypeTags.Clr:
                        return this.Clr;
                    case TypeTags.Function:
                        return this.Func;
                    case TypeTags.Object:
                        return this.Object;
                    case TypeTags.String:
                        return this.String;
                    case TypeTags.SuffixString:
                        return this.SuffixString;
                    case TypeTags.Undefined:
                        return Undefined.Instance;
                    default:
                        return this;
                }
            }
        }

        public static BoxedValue Box(CommonObject value)
        {
            var box = new BoxedValue();
            box.Clr = value;
            box.Tag = TypeTags.Object;
            return box;
        }

        public static BoxedValue Box(FunctionObject value)
        {
            var box = new BoxedValue();
            box.Clr = value;
            box.Tag = TypeTags.Function;
            return box;
        }

        public static BoxedValue Box(string value)
        {
            var box = new BoxedValue();
            box.Clr = value;
            box.Tag = TypeTags.String;
            return box;
        }

        public static BoxedValue Box(SuffixString value)
        {
            var box = new BoxedValue();
            box.Clr = value;
            box.Tag = TypeTags.SuffixString;
            return box;
        }

        public static BoxedValue Box(double value)
        {
            var box = new BoxedValue();
            box.Number = value;
            return box;
        }

        public static BoxedValue Box(bool value)
        {
            var box = new BoxedValue();
            box.Number = value ? TaggedBools.True : TaggedBools.False;
            return box;
        }

        public static BoxedValue Box(object value)
        {
            if (value is double)
                return Box((double)value);
            if (value is int)
                return Box((int)value);
            if (value is bool)
                return Box((bool)value);
            if (value is string)
                return Box((string)value);
            if (value is SuffixString)
                return Box((SuffixString)value);
            if (value is FunctionObject)
                return Box((FunctionObject)value);
            if (value is CommonObject)
                return Box((CommonObject)value);
            if (value is Undefined)
                return Box((Undefined)value);

            var box = new BoxedValue();
            box.Clr = value;
            box.Tag = TypeTags.Clr;
            return box;
        }

        public static BoxedValue Box(object value, uint tag)
        {
            var box = new BoxedValue();
            box.Clr = value;
            box.Tag = tag;
            return box;
        }

        public static BoxedValue Box(Undefined value)
        {
            return Undefined.Boxed;
        }

        public static string FieldOfTag(uint tag)
        {
            switch (tag)
            {
                case TypeTags.Bool:
                    return BoxFields.Bool;
                case TypeTags.Clr:
                    return BoxFields.Clr;
                case TypeTags.Function:
                    return BoxFields.Function;
                case TypeTags.Object:
                    return BoxFields.Object;
                case TypeTags.String:
                    return BoxFields.String;
                case TypeTags.SuffixString:
                    return BoxFields.SuffixString;
                case TypeTags.Undefined:
                    return BoxFields.Undefined;
                case TypeTags.Number:
                    return BoxFields.Number;
                default:
                    throw new ArgumentException(string.Format("Invalid type tag '{0}'", tag));
            }
        }
    }

    public static class BoxingUtils
    {
        public static BoxedValue JsBox(object o)
        {
            if (o is BoxedValue)
                return (BoxedValue)o;

            if (o == null)
                return Environment.BoxedNull;

            var tag = TypeTag.OfType(o.GetType());
            switch (tag)
            {
                case TypeTags.Bool: return BoxedValue.Box((bool)o);
                case TypeTags.Number: return BoxedValue.Box((double)o);
                default: return BoxedValue.Box(o, tag);
            }
        }

        public static object ClrBox(object o)
        {
            if (o is BoxedValue)
                return ((BoxedValue)o).ClrBoxed;

            return o;
        }
    }
}

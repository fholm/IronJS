using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Runtime
{
    public static class TypeTags
    {
        public const uint Box = 0x00000000u;
        public const uint Bool = 0xFFFFFF01u;
        public const uint Number = 0xFFFFFF02u;
        public const uint Clr = 0xFFFFFF03u;
        public const uint String = 0xFFFFFF04u;
        public const uint SuffixString = 0xFFFFFF05u;
        public const uint Undefined = 0xFFFFFF06u;
        public const uint Object = 0xFFFFFF07u;
        public const uint Function = 0xFFFFFF08u;

        private static readonly Dictionary<uint, string> names = new Dictionary<uint, string>
        {
            { Box, "internal" },
            { Bool, "boolean" },
            { Number, "number" },
            { Clr, "clr" },
            { String, "string" },
            { SuffixString, "string" },
            { Undefined, "undefined" },
            { Object, "object" },
            { Function, "function" }
        };

        public static string GetName(uint tag)
        {
            return names[tag];
        }
    }

    public static class TypeTag
    {
        static Dictionary<Type, uint> map
            = new Dictionary<Type, uint>();

        static TypeTag()
        {
            map.Add(typeof(bool), TypeTags.Bool);
            map.Add(typeof(double), TypeTags.Number);
            map.Add(typeof(string), TypeTags.String);
            map.Add(typeof(SuffixString), TypeTags.SuffixString);
            map.Add(typeof(Undefined), TypeTags.Undefined);
            map.Add(typeof(FunctionObject), TypeTags.Function);
            map.Add(typeof(ArrayObject), TypeTags.Object);
            map.Add(typeof(CommonObject), TypeTags.Object);
            map.Add(typeof(ValueObject), TypeTags.Object);
            map.Add(typeof(StringObject), TypeTags.Object);
            map.Add(typeof(NumberObject), TypeTags.Object);
            map.Add(typeof(ErrorObject), TypeTags.Object);
            map.Add(typeof(MathObject), TypeTags.Object);
            map.Add(typeof(BooleanObject), TypeTags.Object);
            map.Add(typeof(RegExpObject), TypeTags.Object);
            map.Add(typeof(DateObject), TypeTags.Object);
        }

        public static uint OfType(Type type)
        {
            uint tag;

            if (map.TryGetValue(type, out tag))
            {
                return tag;
            }

            return  type.IsSubclassOf(typeof(CommonObject)) 
                    ? TypeTags.Object 
                    : TypeTags.Clr;
        }

        public static uint OfObject(object o)
        {
            if (o == null)
                return TypeTags.Clr;

            return OfType(o.GetType());
        }
    }
}

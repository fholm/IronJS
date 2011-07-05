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
}

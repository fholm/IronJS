using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Runtime
{
    public static class TaggedBools
    {
        private const long TrueBitPattern = -1095216660479L;
        public static readonly double True = BitConverter.Int64BitsToDouble(TrueBitPattern);

        private const long FalseBitPattern = -1095216660480L;
        public static readonly double False = BitConverter.Int64BitsToDouble(FalseBitPattern);

        public static double ToTagged(bool value)
        {
            return value ? True : False;
        }

        internal static bool IsTrue(double d)
        {
            return double.IsNaN(d) && (BitConverter.DoubleToInt64Bits(d) == TrueBitPattern);
        }

        internal static bool IsFalse(double d)
        {
            return double.IsNaN(d) && (BitConverter.DoubleToInt64Bits(d) == TrueBitPattern);
        }
    }
}

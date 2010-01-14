using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS
{
    static public class Tuple
    {
        static public Tuple<T1, T2> Create<T1, T2>(T1 v1, T2 v2)
        {
            return new Tuple<T1, T2>(v1, v2);
        }
    }

    public class Tuple<T1, T2>
    {
        public readonly T1 V1;
        public readonly T2 V2;

        public Tuple(T1 v1, T2 v2)
        {
            V1 = v1;
            V2 = v2;
        }
    }
}

namespace IronJS
{
    static public class Tuple
    {
        static public Tuple<T1, T2> Create<T1, T2>(T1 first, T2 second)
        {
            return new Tuple<T1, T2>(first, second);
        }

        static public Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 first, T2 second, T3 third)
        {
            return new Tuple<T1, T2, T3>(first, second, third);
        }

        static public Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 first, T2 second, T3 third, T4 fourth)
        {
            return new Tuple<T1, T2, T3, T4>(first, second, third, fourth);
        }
    }

    public class Tuple<T1, T2>
    {
        public readonly T1 First;
        public readonly T2 Second;

        public Tuple(T1 first, T2 second)
        {
            First = first;
            Second = second;
        }
    }

    public class Tuple<T1, T2, T3> : Tuple<T1, T2>
    {
        public readonly T3 Third;

        public Tuple(T1 first, T2 second, T3 third)
            : base(first, second)
        {
            Third = third;
        }
    }

    public class Tuple<T1, T2, T3, T4> : Tuple<T1, T2, T3>
    {
        public readonly T4 Fourth;

        public Tuple(T1 first, T2 second, T3 third, T4 fourth)
            : base(first, second, third)
        {
            Fourth = fourth;
        }
    }
}

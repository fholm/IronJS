namespace IronJS.Tools {
#if CLR2
	public static class Tuple {
		public static Tuple<T1, T2> Create<T1, T2>(T1 item1, T2 item2) {
			return new Tuple<T1, T2>(item1, item2);
		}

		public static Tuple<T1, T2, T3> Create<T1, T2, T3>(T1 item1, T2 item2, T3 item3) {
			return new Tuple<T1, T2, T3>(item1, item2, item3);
		}

		public static Tuple<T1, T2, T3, T4> Create<T1, T2, T3, T4>(T1 item1, T2 item2, T3 item3, T4 item4) {
			return new Tuple<T1, T2, T3, T4>(item1, item2, item3, item4);
		}
	}

	public class Tuple<T1, T2> {
		public readonly T1 Item1;
		public readonly T2 Item2;

		public Tuple(T1 item1, T2 item2) {
			Item1 = item1;
			Item2 = item2;
		}
	}

	public class Tuple<T1, T2, T3> : Tuple<T1, T2> {
		public readonly T3 Item3;

		public Tuple(T1 item1, T2 item2, T3 item3)
			: base(item1, item2) {
			Item3 = item3;
		}
	}

	public class Tuple<T1, T2, T3, T4> : Tuple<T1, T2, T3> {
		public readonly T4 Item4;

		public Tuple(T1 item1, T2 item2, T3 item3, T4 item4)
			: base(item1, item2, item3) {
			Item4 = item4;
		}
	}
#endif
}

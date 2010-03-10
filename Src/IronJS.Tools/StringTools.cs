using System;
using System.Text;

namespace IronJS.Tools {
	public static class StringTools {
		public static string Repeat(string s, int times) {
			StringBuilder buffer = new StringBuilder();

			for (int i = 0; i < times; ++i)
				buffer.Append(s);

			return buffer.ToString();
		}

		public static string Indent(int times) {
			return new String(' ', times);
		}
	}
}

using System;
using IronJS.Ast.Nodes;
using System.Text;

namespace IronJS.Ast.Tools {
	public static class DisplayTools {
		public static void Print(INode node) {
			Print(node, 0);
		}

		public static void Print(INode node, int indent) {
			var prefix = new String(' ', indent * 2);

			if (node.Children == null || node.Children.Length == 0) {
				Console.WriteLine(prefix + "(" + node + ")");

			} else {
				Console.WriteLine(prefix + "(" + node);

				foreach (INode child in node.Children) {
					if (child != null) {
						Print(child, indent + 1);
					} else {
						Console.WriteLine(prefix + "  ()");
					}
				}

				Console.WriteLine(prefix + ")");
			}
		}

		internal static string AsString(INode node) {
			StringBuilder buffer = new StringBuilder();
			AsString(buffer, node, 0);
			return buffer.ToString();
		}

		static void AsString(StringBuilder buffer, INode node, int indent) {
			var prefix = new String(' ', indent * 2);

			if (node.Children == null || node.Children.Length == 0) {
				buffer.AppendLine(prefix + "(" + node + ")");

			} else {
				buffer.AppendLine(prefix + "(" + node);

				foreach (INode child in node.Children) {
					if (child != null) {
						AsString(buffer, child, indent + 1);
					} else {
						buffer.AppendLine(prefix + "  ()");
					}
				}

				buffer.AppendLine(prefix + ")");
			}
		}
	}
}

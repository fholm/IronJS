using System;
using IronJS.Ast.Nodes;

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
	}
}

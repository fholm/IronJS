using System;
using System.Collections.Generic;
using Antlr.Runtime.Tree;
using Microsoft.Scripting.Utils;

namespace IronJS.Tools {
	static class ITreeTools {
		public static ITree GetChildSafe(ITree that, int n) {
			ITree child = that.GetChild(n);

			if (child == null)
				throw new Ast.AstCompilerError("Expected child");

			if (!child.IsNil && child.Type == 0)
				throw new Ast.AstCompilerError(String.Format("Unexpected '{0}'", child.Text));

			return child;
		}

		public static void EachChild(ITree that, Action<ITree> act) {
			for (int childIndex = 0; childIndex < that.ChildCount; ++childIndex) {
				act(that.GetChild(childIndex));
			}
		}

		public static List<T> Map<T>(ITree that, Func<ITree, T> act) {
			List<T> list = new List<T>();

			for (int childIndex = 0; childIndex < that.ChildCount; ++childIndex) {
				list.Add(act(that.GetChild(childIndex)));
			}

			return list;
		}
	}
}

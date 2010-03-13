using System;
using IronJS.Ast.Nodes;
using IronJS.Runtime.Js;
using Microsoft.Scripting.Utils;

namespace IronJS.Tools {
	static class HashSetTools {
        internal static Type EvalType(HashSet<Type> set) {
			if (set.Count == 1)
				foreach (Type type in set)
					return type;

			return typeof(object);
		}

        internal static Type EvalType(HashSet<INode> set) {
			return HashSetTools.EvalType(
				new HashSet<Type>(
					IEnumerableTools.Map(set, delegate(INode node) { return node.Type; })
				)
			);
		}

        internal static Type EvalType(HashSet<Type> usedAs, HashSet<INode> usedWith) {
            HashSet<Type> set = new HashSet<Type>();

            set.UnionWith(usedAs);
            set.Add(HashSetTools.EvalType(usedWith));

            return HashSetTools.EvalType(set);
        }
    }
}

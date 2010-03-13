using System;
using System.Collections.Generic;
using IronJS.Ast.Nodes;
using IronJS.Tools;
using Microsoft.Scripting.Utils;

namespace IronJS.Ast.Tools {
	internal static class AnalyzeTools {
		internal static INode GetVariable(Stack<Lambda> stack, string name) {
			Lambda current = stack.Peek();
			Stack<Lambda> missingStack = new Stack<Lambda>();

			Variable variable;
			foreach (Lambda function in stack) {
				if (function.Scope.Get(name, out variable)) {
					if (function == current)
						return variable;

                    Local local = variable as Local;
                    if (local != null) {
                        local.MarkAsClosedOver();

						foreach (Lambda traversed in missingStack) {
							traversed.Scope.Add(Node.Enclosed(current, name));
						}

						return current.Scope.Get(name);
					}
				} else {
					missingStack.Push(function);
				}
			}

			return new Global(name);
		}

		internal static void AddClosedType(Stack<Lambda> stack, string name, Type type) {
			Variable variable;
			foreach (Lambda function in stack) {
				if (function.Scope.Get(name, out variable)) {
					if (variable is Local) {
						variable.UsedAs(type);
					}
				}
			}
		}

		internal static bool TypesAreIdentical(params INode[] nodes) {
			if (nodes.Length > 0) {
				Type type = nodes[0].Type;

				for (int index = 0; index < nodes.Length; ++index) {
					if (nodes[index].Type != type) {
						return false;
					}
				}

				return true;
			}

			return false;
		}

		internal static Type EvalTypes(params INode[] nodes) {
			HashSet<Type> set = new HashSet<Type>();

			foreach (INode node in nodes)
				set.Add(node.Type);

			return HashSetTools.EvalType(set);
		}

		internal static void IfIdentifierAssignedFrom(INode node, INode value) {
			Variable variable = node as Variable;

			if (variable != null) {
				variable.AssignedFrom(value);
			}
		}

		internal static void IfIdentiferUsedAs(INode node, Type type) {
			Variable variable = node as Variable;

			if (variable != null) {
				variable.UsedAs(type);
			}
		}
	}
}
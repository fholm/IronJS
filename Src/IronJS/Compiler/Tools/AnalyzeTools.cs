using System;
using System.Collections.Generic;
using IronJS.Compiler.Ast;
using IronJS.Tools;
using Microsoft.Scripting.Utils;

namespace IronJS.Compiler.Tools {
	internal static class AnalyzeTools {
		internal static INode GetVariable(Stack<Function> stack, string name) {
			Function current = stack.Peek();
			Stack<Function> missingStack = new Stack<Function>();

			Variable variable;
			foreach (Function function in stack) {
				if (function.Variables.TryGetValue(name, out variable)) {
					if (function == current)
						return variable;

					if (variable is Local) {
						variable.MarkAsClosedOver();

						foreach (Function traversed in missingStack) {
							traversed[name] = new Closed(traversed, name);
						}

						return current.Variables[name];
					}
				} else {
					missingStack.Push(function);
				}
			}

			return new Global(name);
		}

		internal static void AddClosedType(Stack<Function> stack, string name, Type type) {
			Variable variable;
			foreach (Function function in stack) {
				if (function.Variables.TryGetValue(name, out variable)) {
					if (variable is Local) {
						variable.UsedAs(type);
					}
				}
			}
		}

		internal static bool IdenticalTypes(params INode[] nodes) {
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
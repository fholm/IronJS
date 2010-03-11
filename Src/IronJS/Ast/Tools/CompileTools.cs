using IronJS.Ast.Nodes;
using IronJS.Runtime.Js;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Tools {
	using AstUtils = Utils;
	using Et = Expression;

	internal static partial class CompileTools {
		internal static Et Assign(Lambda func, INode target, Et value) {

			Global global = target as Global;
			if (global != null) {
				return Et.Call(
					Globals(func), 
					typeof(Obj).GetMethod("Set"), 
					CompileTools.Constant(global.Name), 
					value
				);
			} else {
				return AstUtils.Empty();
			}
		}

		internal static Et Constant(object obj) {
			return Et.Constant(obj);
		}

		internal static Et Globals(Lambda func) {
			return Et.Field(func.Children[1].Compile(func), "Globals");
		}

		internal static Et Context(Lambda func) {
			return Et.Field(func.Children[1].Compile(func), "Context");
		}

		internal static bool IsGlobal(INode node) {
			Assign asn = node as Assign;

			if (asn != null) {
				return (asn.Target as Global) != null;
			}

			return (node as Global) != null;
		}

		internal static bool As<T>(INode node, out T casted) where T : class {
			casted = node as T;
			return casted != null;
		}
	}
}

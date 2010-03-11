using IronJS.Ast.Nodes;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Tools {
	using AstUtils = Utils;
	using Et = Expression;

	internal static partial class CompileTools {
		internal static Et Assign(Lambda func, INode Target, Et value) {
			return AstUtils.Empty();
		}

		internal static Et Globals(Lambda func) {
			return Et.Field(func.Children[1].Compile(func), "Globals");
		}

		internal static Et Context(Lambda func) {
			return Et.Field(func.Children[1].Compile(func), "Context");
		}
	}
}

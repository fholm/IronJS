using IronJS.Ast.Nodes;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Tools {
	using AstUtils = Microsoft.Scripting.Ast.Utils;
	using Et = Expression;

	internal static partial class CompileTools {
		internal static Et Assign(Lambda func, INode Target, Et value) {
			return AstUtils.Empty();
		}
	}
}

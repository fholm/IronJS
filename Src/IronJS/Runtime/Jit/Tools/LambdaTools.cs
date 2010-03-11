using System;
using IronJS.Ast.Nodes;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Runtime.Jit.Tools {
	using Et = Expression;

	static class LambdaTools {
		static internal void SetupParameterTypes(Lambda lambda, Type[] types) {
			for (int i = 1; i <= lambda.ParamsCount; ++i) {
				(lambda.Children[i] as Parameter).ForceType(types[i - 1]);
			}
		}

		static internal void ResetParameterTypes(Lambda lambda) {
			for (int i = 1; i <= lambda.ParamsCount; ++i) {
				(lambda.Children[i] as Parameter).ForceType(null);
			}
		}

		internal static void SetupVariables(Lambda lambda) {
			foreach (Variable variable in lambda.Variables.Values) {
				variable.Setup();
			}
		}

		internal static void ResetVariables(Lambda lambda) {
			foreach (Variable variable in lambda.Variables.Values) {
				variable.Clear();
			}
		}

		internal static void SetupReturnLabel(Lambda lambda) {
			lambda.ReturnLabel = Et.Label(lambda.ReturnType, "~return");
		}

		internal static void ResetReturnLabel(Lambda lambda) {
			lambda.ReturnLabel = null;
		}
	}
}

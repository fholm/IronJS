using System;
using IronJS.Ast.Nodes;

namespace IronJS.Runtime.Jit.Tools {
	static class LambdaTools {
		static internal void SetParameterTypes(Lambda lambda, Type[] types) {
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

		internal static void ClearVariables(Lambda lambda) {
			foreach (Variable variable in lambda.Variables.Values) {
				variable.Clear();
			}
		}
	}
}

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
	}
}

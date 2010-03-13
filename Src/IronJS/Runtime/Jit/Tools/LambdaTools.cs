using System;
using IronJS.Ast;
using IronJS.Ast.Nodes;
using IronJS.Tools;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Runtime.Jit.Tools {
	using Et = Expression;

	static class LambdaTools {
		static internal void SetupParameterTypes(Lambda lambda, Type[] types) {
			for (int i = 1; i <= lambda.Scope.ParameterCount; ++i) {
                (lambda.Children[i] as Param).InType = types[i - 1];
			}
		}

		internal static void SetupVariables(Lambda lambda) {
			foreach (IVariable variable in lambda.Scope) {
				variable.Setup();
			}
		}

		internal static void ResetVariables(Lambda lambda) {
			foreach (IVariable variable in lambda.Scope) {
				variable.Clear();
			}
		}

		internal static void SetupReturnLabel(Lambda lambda) {
			lambda.ReturnLabel = Et.Label(lambda.ReturnType, "~return");
		}

		internal static void ResetReturnLabel(Lambda lambda) {
			lambda.ReturnLabel = null;
		}

        internal static Type BuildDelegateType(Lambda lambda, Type[] paramTypes) {
            Type[] evaledTypes = new Type[paramTypes.Length];

            for (int i = 0; i < paramTypes.Length; ++i) {
                lambda.Scope.Parameters[i].InType = paramTypes[i];
                evaledTypes[i] = lambda.Scope.Parameters[i].Type;
            }

            return DelegateTools.BuildFuncType(evaledTypes);
        }
	}
}

using System;
using IronJS.Ast.Nodes;
using IronJS.Tools;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Runtime.Jit.Tools {
    static class LambdaTools {
        internal static Type BuildDelegateType(Lambda lambda) {
            Type[] types = new Type[lambda.Scope.Parameters.Count];

            int n = 0;
            foreach (Param param in lambda.Scope.Parameters) {
                types[n++] = param.Type;
            }

            return DelegateTools.BuildFuncType(types);
        }
	}
}

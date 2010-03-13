/* ***************************************************************************************
 *
 * Copyright (c) Fredrik Holmström
 *
 * This source code is subject to terms and conditions of the Microsoft Public License. 
 * A copy of the license can be found in the License.html file at the root of this 
 * distribution. If you cannot locate the  Microsoft Public License, please send an 
 * email to fredrik.johan.holmstrom@gmail.com. By using this source code in any fashion, 
 * you are agreeing to be bound by the terms of the Microsoft Public License.
 *
 * You must not remove this notice, or any other, from this software.
 *
 * ***************************************************************************************/
using System;
using System.Collections.Generic;
using IronJS.Ast.Nodes;
using IronJS.Tools;
using IronJS.Runtime.Jit.Tools;
using Microsoft.Scripting.Utils;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Runtime.Jit {
	using Et = Expression;
	using EtParam = ParameterExpression;
	using AstUtils = Microsoft.Scripting.Ast.Utils;

	public class Compiler {
		public Delegate Compile(Lambda func, Type[] paramTypes) {
            // Setup parameter types
            for (int i = 0; i < paramTypes.Length; ++i) {
                func.Scope.Parameters[i].InType = paramTypes[i];
            }

            // Construct delegate type
            Type funcType = LambdaTools.BuildDelegateType(func);

            Delegate compiled;
            if (!func.JitCache.TryGet(funcType, out compiled)) {
                // Initialize variables
                foreach (IVariable variable in func.Scope) {
                    variable.Setup();
                }

                // Setup return label
                func.ReturnLabel = Et.Label(func.ReturnType, "~return");

                // Build locals init block
                List<EtParam> localsExprs;
                Et initBlock = BuildLocalsInitBlock(func, out localsExprs);

                // DLR Lambda expression
                LambdaExpression lambda = Et.Lambda(
                    funcType,
                    Et.Block(
                        localsExprs,
                        initBlock,
                        func.Body.Compile(func),
                        Et.Label(
                            func.ReturnLabel, Et.Default(func.ReturnType)
                        )
                    ),
                    IEnumerableTools.Map(func.Scope.Parameters, delegate(Param param) {
                        return (EtParam) param.Compile(func);
                    })
                );

                // Compile and store in cache
                compiled = func.JitCache.Save(
                    funcType, lambda.Compile()
                );

                // Setup return label
                func.ReturnLabel = null;

                // Reset variable expressions
                foreach (IVariable variable in func.Scope) {
                    variable.Clear();
                }
            }

			return compiled;
		}

		Et BuildLocalsInitBlock(Lambda func, out List<EtParam> localsExprs) {
			localsExprs = new List<EtParam>();

			EtParam paramExpr;
			List<Et> initExprs = new List<Expression>();
			foreach (Local variable in func.Scope.Locals) {
				paramExpr = (EtParam) variable.Compile(func);

				if(!variable.Type.IsValueType)
					initExprs.Add(Et.Assign(paramExpr, AstTools.New(variable.Type)));

				localsExprs.Add(paramExpr);
			}

			if (initExprs.Count == 0)
				return AstUtils.Empty();

			return Et.Block(initExprs);
		}


	}
}

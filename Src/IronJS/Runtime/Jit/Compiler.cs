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
using IronJS.Ast.Tools;
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

            DisplayTools.Print(func);

            Delegate compiled;
            if (!func.JitCache.TryGet(funcType, out compiled)) {
                // Initialize variables
                foreach (IVariable variable in func.Scope) {
                    variable.Setup();
                }

                // Setup return label
                func.ReturnLabel = Et.Label(func.ReturnType, "~return");

                // 
                List<Et> initExprs = new List<Et>();
                List<EtParam> localsExprs = new List<EtParam>();

                // 
                AddLocals(func, initExprs, localsExprs);

                // 
                Et initBlock = initExprs.Count == 0
                               ? (Et) AstUtils.Empty()
                               : (Et) Et.Block(initExprs);

                // DLR Lambda expression
                LambdaExpression lambda = Et.Lambda(
                    funcType,
                    Et.Block(
                        localsExprs,
                        initBlock, //hack :(
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

                // Reset variables
                foreach (IVariable variable in func.Scope) {
                    variable.Reset();
                }
            }

			return compiled;
		}

        void AddLocals(Lambda func, List<Et> initExprs, List<EtParam> localsExprs) {
            EtParam localExpr;
            foreach (Local variable in func.Scope.Locals) {
                localExpr = (EtParam)variable.Compile(func);

                if (variable.IsClosedOver)
                    initExprs.Add(Et.Assign(localExpr, AstTools.New(localExpr.Type)));

                localsExprs.Add(localExpr);
            }
        }
	}
}

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
		public TFunc Compile<TFunc>(Lambda func) where TFunc : class {
			return (TFunc)Compile(typeof(TFunc), func);
		}
							
		public object /*hack*/ Compile(Type funcType, Lambda func) {
			Type[] types = funcType.GetGenericArguments();
			Type[] paramTypes = ArrayUtils.RemoveLast(types);

			LambdaTools.SetupParameterTypes(func, paramTypes);
			LambdaTools.SetupVariables(func);
			LambdaTools.SetupReturnLabel(func);

			List<EtParam> localsExprs;
			Et initBlock = BuildLocalsInitBlock(func, out localsExprs);

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
				ArrayTools.Map(
					ArrayTools.DropFirstAndLast(func.Children) /*remove name and body nodes*/,
					delegate(INode node) {
						return node.Compile(func) as ParameterExpression;
					}
				)
			);

			Delegate compiled = lambda.Compile();

			LambdaTools.ResetReturnLabel(func);
			LambdaTools.ResetVariables(func);

			return compiled;
		}

		Et BuildLocalsInitBlock(Lambda func, out List<EtParam> localsExprs) {
			localsExprs = new List<EtParam>();

			EtParam param;
			List<Et> initExprs = new List<Expression>();
			foreach (Local variable in func.Scope.Locals) {
				param = (EtParam) variable.Compile(func);

				if(!variable.Type.IsValueType)
					initExprs.Add(Et.Assign(param, AstTools.New(variable.Type)));

				localsExprs.Add(param);
			}

			if (initExprs.Count == 0)
				return AstUtils.Empty();

			return Et.Block(initExprs);
		}


	}
}

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

	public class Compiler {
		public TFunc Compile<TFunc>(Lambda lambda) where TFunc : class {
			return (TFunc)Compile(typeof(TFunc), lambda);
		}

		public object /*hack*/ Compile(Type funcType, Lambda lambda) {
			Type[] types = funcType.GetGenericArguments();
			Type[] paramTypes = ArrayUtils.RemoveLast(types);

			LambdaTools.SetParameterTypes(lambda, paramTypes);
			LambdaTools.SetupVariables(lambda);

			LambdaExpression lambdaExpr = Et.Lambda(
				funcType, lambda.Body.Compile(lambda), "~file",
				ArrayTools.Map(
					ArrayTools.DropFirstAndLast(lambda.Children) /*remove name and body nodes*/,
					delegate(INode node) {
						return node.Compile(lambda) as ParameterExpression;
					}
				)
			);
			Delegate compiled = lambdaExpr.Compile();

			LambdaTools.ClearVariables(lambda);
			LambdaTools.ResetParameterTypes(lambda);

			DisplayTools.Print(lambda);
			return compiled;
		}
	}
}

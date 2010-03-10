using System;
using System.Dynamic;
using System.Reflection;
using System.Collections.Generic;
using IronJS.Tools;
using IronJS.Ast.Nodes;
using IronJS.Runtime.Js;
using IronJS.Runtime.Binders;
using Microsoft.Scripting.Utils;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Ast.Tools {
	using Et = Expression;
	using AstUtils = Microsoft.Scripting.Ast.Utils;
	using EtParam = ParameterExpression;

	internal static partial class CompileTools {
		internal static Et Call0(Lambda func, INode target) {
			EtParam tmpObject = Et.Variable(typeof(object), "~tmpObject");
			EtParam tmpFunc = Et.Variable(typeof(Function), "~tmpFunc");

			/*
			 * This constructs a block that looks like this
			 * 
			 * var tmpFunc = tmpObject as IjsFunc;
			 * if(tmp != null) {
			 *		if(tmpFunc.Func0 == null) {
			 *			tmpFunc.Compile0();
			 *		}
			 *		
			 *		tmpFunc.Func0(tmpFunc.Closure);
			 * } else {
			 *		// Dynamic Expression
			 * }
			 * */

			return Et.Block(
				new[] { tmpObject, tmpFunc },
				Et.Assign(tmpObject, target.Compile(func)),
				Et.Assign(
					tmpFunc,
					Et.TypeAs(tmpObject, typeof(Function))
				),
				Et.Condition(
					Et.NotEqual(tmpFunc, Et.Default(typeof(Function))),
					Et.Block(
						Et.IfThen(
							Et.Equal(Et.Field(tmpFunc, "Func0"), Et.Constant(null)),
							Et.Call(tmpFunc, typeof(Function).GetMethod("Compile0"))
						),
						Et.Invoke(Et.Field(tmpFunc, "Func0"), Et.Field(tmpFunc, "Closure"))
					),
					Et.Dynamic(
						new CallBinder(new CallInfo(0)),
						Types.Dynamic,
						tmpObject
					)
				)
			);
		}

		internal static Et CallN(Lambda func, INode target, IEnumerable<INode> argsList) {
			// Build the args array
			Et[] args = IEnumerableTools.Map(argsList, delegate(INode node) {
				return node.Compile(func);
			});

			// Construct the proxy type
			Type proxyType = BuildCallProxyType(args);

			// All other types we need
			Type funcType = typeof(Function);
			Type delegateType = proxyType.GetField("Delegate").FieldType;
			Type guardType = proxyType.GetField("Guard").FieldType;

			// All expressions we need through out the building
			Et callExpr = null;
			Et funcField = Et.Field(callExpr, "Func");
			Et delegateField = Et.Field(callExpr, "Delegate");
			Et guardField = Et.Field(callExpr, "Guard");
			Et closureField = Et.Field(funcField, "Closure");

			// Temporary variables
			EtParam tmpObject = Et.Variable(Types.Dynamic, "~tmpObject");
			EtParam tmpFunc = Et.Variable(funcType, "~tmpFunc");
			EtParam tmpGuard = Et.Variable(guardType, "~tmpGuard");

			/*
			 * This constructs a block that looks like this
			 * 
			 * tmpFunc = tmpObject as IjsFunc;
			 * if(tmpFunc != null) {
			 *		if(tmpFunc == proxy.Func) {
			 *			if(proxy.Guard(arg1, arg2, ...)) {
			 *				proxy.Delegate(arg1, arg2, ...);
			 *			} else {
			 *				proxy.Delegate = tmpFunc.CompileN<TFunc, TGuard>([arg1, arg2, ...], out tmpGuard);
			 *				proxy.Guard = tmpGuard;
			 *				proxy.Delegate(tmpFunc.Closure, arg1, arg2, ...);
			 *			}
			 *		} else {
			 *			proxy.Func = tmpFunc;
			 *			proxy.Delegate = tmpFunc.CompileN<TFunc, TGuard>([arg1, arg2, ...], out tmpGuard);
			 *			proxy.Guard = tmpGuard;
			 *			proxy.Delegate(tmpFunc.Closure, arg1, arg2, ...);
			 *		}
			 * } else {
			 *		// Dynamic Expression
			 * }
			 * */

			return Et.Block(
				new[] { tmpObject, tmpFunc },
				Et.Assign(tmpObject, target.Compile(func)),
				Et.Assign(tmpFunc, Et.TypeAs(tmpObject, funcType)),
				Et.Condition(
					Et.NotEqual(tmpFunc, Et.Default(funcType)),
					Et.Block(
						new[] { tmpGuard },
						Et.Condition(
							Et.Equal(tmpFunc, funcField),
							Et.Condition(
								Et.Invoke(guardField, args),
								Et.Invoke(delegateField, ArrayUtils.Insert(closureField, args)),
								Et.Block(
									BuildUpdateExpr(delegateField, funcField, tmpGuard, delegateType, guardType, args),
									Et.Assign(guardField, tmpGuard),
									Et.Invoke(delegateField, ArrayUtils.Insert(closureField, args))
								)
							),
							Et.Block(
								Et.Assign(funcField, tmpFunc),
								BuildUpdateExpr(delegateField, funcField, tmpGuard, delegateType, guardType, args),
								Et.Assign(guardField, tmpGuard),
								Et.Invoke(delegateField, ArrayUtils.Insert(closureField, args))
							)
						)
					),
					Et.Dynamic(
						new CallBinder(new CallInfo(args.Length)),
						Types.Dynamic,
						ArrayUtils.Insert(tmpObject, args)
					)
				)
			);
		}

		internal static Et Assign(Lambda func, INode Target, Et value) {
			return AstUtils.Empty();
		}

		static Et BuildUpdateExpr(Expression delegateField, Expression funcField, ParameterExpression tmpGuard,
			Type delegateType, Type guardType, Expression[] args) {
			return Et.Assign(
				delegateField,
				Et.Call(
					funcField,
					typeof(Function).GetMethod("CompileN").MakeGenericMethod(delegateType, guardType),
					AstUtils.NewArrayHelper(typeof(object), args),
					tmpGuard
				)
			);
		}
	}
}

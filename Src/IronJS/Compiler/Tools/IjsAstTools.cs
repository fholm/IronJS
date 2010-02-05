using System;
using System.Dynamic;
using System.Reflection;
using System.Collections.Generic;
using IronJS.Tools;
using IronJS.Compiler.Ast;
using IronJS.Runtime2.Js;
using IronJS.Runtime2.Binders;
using IronJS.Runtime2.Js.Proxies;
using Microsoft.Scripting.Utils;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Tools
{
    using Et = Expression;
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using EtParam = ParameterExpression;

    internal static class IjsAstTools
    {
		internal static Et Call0(FuncNode func, INode target)
		{
			EtParam tmpObject = Et.Variable(typeof(object), "__tmpObject__");
			EtParam tmpFunc = Et.Variable(typeof(IjsFunc), "__tmpFunc__");

			return Et.Block(
				new[] { tmpObject, tmpFunc },
				Et.Assign(tmpObject, target.Compile(func)),
				Et.Assign(
					tmpFunc,
					Et.TypeAs(tmpObject, typeof(IjsFunc))
				),
				Et.Condition(
					Et.NotEqual(tmpFunc, Et.Default(typeof(IjsFunc))),
					Et.Block(
						Et.IfThen(
							Et.Equal(Et.Field(tmpFunc, "Func0"), Et.Constant(null)),
							Et.Call(tmpFunc, typeof(IjsFunc).GetMethod("Compile0"))
						),
						Et.Invoke(
							Et.Field(tmpFunc, "Func0"),
							Et.Field(tmpFunc, "Closure")
						)
					),
					Et.Dynamic(
						new IjsInvokeBinder(new CallInfo(0)),
						IjsTypes.Dynamic,
						tmpObject
					)
				)
			);
		}

		internal static Et CallN(FuncNode func, INode target, IEnumerable<INode> argsList)
		{
			// Build the args array
			Et[] args = IEnumerableTools.Map(argsList, delegate(INode node) {
				return node.Compile(func);
			});

			// Construct the proxy type
			Type proxyType = typeof(IjsCall1<>).MakeGenericType(
				IEnumerableTools.Map(args, delegate(Expression expr) {
					return expr.Type;
				})
			);

			// All other types we need
			Type funcType = typeof(IjsFunc);
			Type delegateType = proxyType.GetField("Delegate").FieldType;
			Type guardType = proxyType.GetField("Guard").FieldType;

			// All expressions we need through out the building
			Et callExpr = func.GetCallProxy(proxyType);
			Et funcField = Et.Field(callExpr, "Func");
			Et delegateField = Et.Field(callExpr, "Delegate");
			Et guardField = Et.Field(callExpr, "Guard");
			Et closureField = Et.Field(funcField, "Closure");

			// Temporary variables
			EtParam tmpObject = Et.Variable(IjsTypes.Dynamic, "__tmpObject__");
			EtParam tmpFunc = Et.Variable(funcType, "__tmpFunc__");
			EtParam tmpGuard = Et.Variable(guardType, "__tmpGuard__");

			/*
			 * This constructs a block that looks like this
			 * 
			 * tmpFunc = tmpObject as IjsFunc;
			 * if(tmpFunc != null) {
			 *		if(tmpFunc == proxy.Func) {
			 *			if(proxy.Guard(arg1, arg2, ...)) {
			 *				proxy.Delegate(arg1, arg2, ...);
			 *			} else {
			 *				proxy.Delegate = tmpFunc.CompileN([arg1, arg2, ...], out tmpGuard);
			 *				proxy.Guard = tmpGuard;
			 *				proxy.Delegate(arg1, arg2, ...);
			 *			}
			 *		} else {
			 *			proxy.Func = tmpFunc;
			 *			proxy.Delegate = tmpFunc.CompileN([arg1, arg2, ...], out tmpGuard);
			 *			proxy.Guard = tmpGuard;
			 *			proxy.Delegate(arg1, arg2, ...);
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
									Et.Assign(
										delegateField,
										Et.Call(
											funcField,
											typeof(IjsFunc).GetMethod("CompileN").MakeGenericMethod(delegateType, guardType),
											AstUtils.NewArrayHelper(typeof(object), args),
											tmpGuard
										)
									),
									Et.Assign(guardField, tmpGuard),
									Et.Invoke(delegateField, ArrayUtils.Insert(closureField, args))
								)
							),
							Et.Block(
								Et.Assign(funcField, tmpFunc),
								Et.Assign(
									delegateField,
									Et.Call(
										funcField,
										typeof(IjsFunc).GetMethod("CompileN").MakeGenericMethod(delegateType, guardType),
										AstUtils.NewArrayHelper(typeof(object), args),
										tmpGuard
									)
								),
								Et.Assign(guardField, tmpGuard),
								Et.Invoke(delegateField, ArrayUtils.Insert(closureField, args))
							)
						)
					),
					Et.Dynamic(
						new IjsInvokeBinder(new CallInfo(args.Length)),
						IjsTypes.Dynamic,
						ArrayUtils.Insert(tmpObject, args)
					)
				)
			);
		}

        internal static Et Assign(FuncNode func, INode Target, Et value)
        {
            IdentifierNode idNode = Target as IdentifierNode;
            if (idNode != null)
            {
                IjsIVar varInfo = idNode.VarInfo;

                if (func.IsGlobal(varInfo))
                {
                    return Et.Call(
                        func.GlobalField,
                        typeof(IjsObj).GetMethod("Set"),
                        AstTools.Constant(idNode.Name),
						AstTools.Box(value)
                    );
                }
                else if(func.IsLocal(idNode.VarInfo))
                {
                    IjsLocalVar localVarInfo = varInfo as IjsLocalVar;

                    if (idNode.IsDefinition)
                    {
                        localVarInfo.Expr = Et.Variable(
                            localVarInfo.ExprType,
                            idNode.Name
                        );
                    }

                    return Et.Assign(
                        localVarInfo.Expr,
                        value
                    );
                }
            }

            throw new NotImplementedException();
        }
    }
}

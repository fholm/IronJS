using System;
using IronJS.Tools;
using IronJS.Compiler.Ast;
using IronJS.Runtime2.Js;
using System.Reflection;

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
	using IronJS.Runtime2.Binders;
	using System.Dynamic;

    internal static class IjsAstTools
    {
		internal static Et CallNoArgs(INode target, FuncNode func)
		{
			EtParam tmpObject = Et.Variable(typeof(object), "__tmpObject__");
			EtParam tmpFunc = Et.Variable(typeof(IjsFunc), "__tmpFunc__");

			return Et.Block(
				new[] { tmpObject, tmpFunc },
				Et.Assign(
					tmpObject,
					target.Compile(func)
				),
				Et.Assign(
					tmpFunc,
					Et.ConvertChecked(tmpObject, typeof(IjsFunc))
				),
				Et.Condition(
					Et.NotEqual(tmpFunc, Et.Default(typeof(IjsFunc))),
					Et.Block(
						Et.IfThen(
							Et.Equal(
								Et.Field(tmpFunc, "Func0"),
								Et.Constant(null)
							),
							Et.Call(
								tmpFunc,
								typeof(IjsFunc).GetMethod("Compile0")
							)
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

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

    internal static class IjsEtGenUtils
    {
        internal static Et Constant<T>(T value)
        {
            return Et.Constant(value, typeof(T));
        }

        internal static Et Box(Et value)
        {
            if (value.Type == typeof(void))
                return Et.Block(
                    value,
                    Et.Default(typeof(object))
                );

            return Et.Convert(value, IjsTypes.Dynamic);
        }

        internal static Et New(Type type, params Et[] parameters)
        {
			ConstructorInfo ctor = type.GetConstructor(
				ArrayTools.Map(parameters, delegate(Et expr) {
					return expr.Type;
				});
			);

            if (ctor == null)
                throw new NotImplementedException("No constructor taking these parameters exist");

            return AstUtils.SimpleNewHelper(ctor, parameters);
        }

        internal static Et Assign(FuncNode func, Ast.INode Target, Et value)
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
                        Constant(idNode.Name),
                        Box(value)
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

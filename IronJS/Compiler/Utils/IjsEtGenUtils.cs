using System;
using System.Linq;
using System.Collections.Generic;
using IronJS.Compiler.Ast;
using IronJS.Runtime.Js;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Et = System.Linq.Expressions.Expression;
using IronJS.Runtime2.Js;

namespace IronJS.Compiler.Utils
{
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
            var ctor = type.GetConstructor(
                parameters.Select(x => x.Type).ToArray()
            );

            if (ctor == null)
                throw new NotImplementedException("No constructor taking these parameters exist");

            return AstUtils.SimpleNewHelper(ctor, parameters);
        }

        internal static Et Assign(FuncNode func, Ast.INode Target, Et value)
        {
            var idNode = Target as IdentifierNode;
            if (idNode != null)
            {
                var varInfo = idNode.VarInfo;

                if (varInfo.IsGlobal)
                {
                    return Et.Call(
                        func.GlobalField,
                        typeof(IjsObj).GetMethod("Set"),
                        Constant(idNode.Name),
                        Box(value)
                    );
                }
                else
                {
                    if (varInfo.IsClosedOver)
                    {

                    }
                    else
                    {

                    }
                }
            }

            throw new NotImplementedException();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Dynamic;
using Microsoft.Scripting.Utils;
using IronJS.Runtime.Binders;

namespace IronJS.Runtime.Utils
{
    using Et = System.Linq.Expressions.Expression;
    using Js = IronJS.Runtime.Js;
    using AstUtils = Microsoft.Scripting.Ast.Utils;

    static class JsUtils
    {
        internal static Et CreateNewObjectExpr(Context context)
        {
            return Et.Call(
                Et.Constant(context),
                "CreateObject",
                Type.EmptyTypes
            );
        }

        /// <summary>
        /// 10.1.8
        /// Implements the arguments object
        /// </summary>
        /// <param name="target"></param>
        /// <param name="argsExprs"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        internal static Et CreateJsArgsObj(Et target, Et[] argsExprs, Context context)
        {
            var tmp = Et.Variable(typeof(Js.Obj), "__tmp.arguments__");
            var exprs = new List<Et>();

            exprs.Add(
                Et.Assign(
                    tmp,
                    CreateNewObjectExpr(context)
                )
            );

            exprs.Add(
                Et.Dynamic(
                    new JsSetMemberBinder("length"),
                    typeof(object),
                    tmp,
                    EtUtils.Box(Et.Constant(argsExprs.Length))
                )
            );

            exprs.Add(
                Et.Dynamic(
                    new JsSetMemberBinder("callee"),
                    typeof(object),
                    tmp,
                    target
                )
            );

            for(var i = 0; i < argsExprs.Length; ++i)
            {
                exprs.Add(
                    Et.Dynamic(
                        new JsSetMemberBinder(i, Js.PropertyAttrs.DontEnum),
                        typeof(object),
                        tmp,
                        argsExprs[i]
                    )
                );
            }

            exprs.Add(tmp);

            return Et.Block(
                new[] { tmp },
                exprs
            );
        }
    }
}

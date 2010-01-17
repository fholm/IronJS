using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using IronJS.Runtime.Js;
using Microsoft.Scripting.Utils;

namespace IronJS.Runtime.Js.Utils
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;
    using ParamEt = System.Linq.Expressions.ParameterExpression;
    using AstUtils = Microsoft.Scripting.Ast.Utils;

    public static class FrameEtUtils
    {
        internal static Et Push(Et frame, object name, Et value)
        {
            return Et.Call(
                frame,
                typeof(IObj).GetMethod("Put"),
                Et.Constant(name),
                value
            );
        }

        internal static Et Pull(Et frame, object name)
        {
            return Et.Call(
                frame,
                typeof(IObj).GetMethod("Get"),
                Et.Constant(name)
            );
        }

        internal static Et Enter(Et target, Et parent)
        {
            return Et.Assign(
                target,
                AstUtils.SimpleNewHelper(
                    typeof(Frame).GetConstructor(new[] { typeof(IObj), typeof(Context) }),
                    parent,
                    Et.Property(
                        parent,
                        "Context"
                    )
                )
            );
        }

        internal static Et Exit(Et target, Et parent)
        {
            return Et.Assign(
                target,
                Et.Property(
                    parent,
                    "Prototype"
                )
            );
        }
    }
}

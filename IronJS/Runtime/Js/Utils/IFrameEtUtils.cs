using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using IronJS.Runtime.Js;

namespace IronJS.Runtime.Js.Utils
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;

    public static class IFrameEtUtils
    {
        internal static Et Push(Et frame, string name, Et value, VarType type)
        {
            return Et.Call(
                frame,
                typeof(IFrame).GetMethod("Push"),
                Et.Constant(name),
                value,
                Et.Constant(type)
            );
        }

        internal static Et Pull(Et frame, string name, GetType type)
        {
            return Et.Call(
                frame,
                typeof(IFrame).GetMethod("Pull"),
                Et.Constant(name),
                Et.Constant(type)
            );
        }

        internal static Et Pull<T>(Et frame, string name, GetType type)
        {
            return Et.Convert(
                IFrameEtUtils.Pull(
                    frame,
                    name,
                    type
                ),
                typeof(T)
            );
        }

        internal static Et Enter(Et target, Et parent)
        {
            return Et.Assign(
                target,
                Et.Call(
                    parent,
                    typeof(IFrame).GetMethod("Enter", Type.EmptyTypes)
                )
            );
        }

        internal static Et Exit(Et target, Et parent)
        {
            return Et.Assign(
                target,
                Et.Call(
                    parent,
                    typeof(IFrame).GetMethod("Exit", Type.EmptyTypes)
                )
            );
        }
    }
}

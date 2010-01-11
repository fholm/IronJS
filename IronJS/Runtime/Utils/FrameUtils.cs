using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using IronJS.Runtime.Js;

namespace IronJS.Runtime.Utils
{
    using Et = System.Linq.Expressions.Expression;
    using Meta = System.Dynamic.DynamicMetaObject;

    public static class FrameUtils
    {
        internal static Et Push(Et frame, string name, Et value, VarType type)
        {
            return Et.Call(
                frame,
                typeof(Frame).GetMethod("Push"),
                Et.Constant(name),
                value,
                Et.Constant(type)
            );
        }

        internal static Et Pull(Et frame, string name)
        {
            return Et.Call(
                frame,
                typeof(Frame).GetMethod("Pull"),
                Et.Constant(name)
            );
        }

        internal static Et Pull<T>(Et frame, string name)
        {
            return Et.Convert(
                FrameUtils.Pull(
                    frame,
                    name
                ),
                typeof(T)
            );
        }

        internal static Et EnterFrame(Et target, Et parent)
        {
            return Et.Assign(
                target,
                Et.Call(
                    parent,
                    typeof(Frame).GetMethod("Enter", Type.EmptyTypes)
                )
            );
        }
    }
}

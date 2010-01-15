using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using IronJS.Extensions;
using IronJS.Runtime;
using IronJS.Runtime.Binders;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Microsoft.Scripting.Utils;

namespace IronJS.Runtime.Js
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = System.Linq.Expressions.Expression;

    public enum InvokeFlag { Method, Function }

    public interface IFunction : IObj
    {
        // 8.6.2
        IFrame Frame { get; }   // [[Scope]]
        Lambda Lambda { get; }  // [[Call]]

        // 8.6.2
        IObj Construct();   // [[Construct]]
    }

    public static class IFunctionUtils
    {
        public static Et Frame(Et obj)
        {
            return Et.Property(
                EtUtils.Cast<IFunction>(obj),
                "Frame"
            );
        }

        public static Et Lambda(Et obj)
        {
            return Et.Property(
                EtUtils.Cast<IFunction>(obj),
                "Lambda"
            );
        }

        public static Et Delegate(Et obj)
        {
            return Et.Field(
                Lambda(obj),
                "Delegate"
            );
        }

        public static Et Call(Et obj, Et frame)
        {
            return Et.Call(
                Delegate(obj),
                typeof(Func<IFrame, object>).GetMethod("Invoke"),
                frame
            );
        }
    }
}

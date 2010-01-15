using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Js;

namespace IronJS.Runtime.Utils
{
    using Et = System.Linq.Expressions.Expression;
    using AstUtils = Microsoft.Scripting.Ast.Utils;

    static class ObjUtils
    {
        static internal Et SetOwnProperty(Et target, object name, Et value)
        {
            return Et.Call(
                target,
                typeof(IObj).GetMethod("SetOwnProperty"),
                EtUtils.Box(Et.Constant(name)),
                value
            );
        }

        static internal Et CreateNew()
        {
            return AstUtils.SimpleNewHelper(
                typeof(IObj).GetConstructor(Type.EmptyTypes)
            );
        }
    }
}

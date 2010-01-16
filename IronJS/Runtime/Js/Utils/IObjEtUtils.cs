using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Js.Utils
{
    using Et = System.Linq.Expressions.Expression;
    using AstUtils = Microsoft.Scripting.Ast.Utils;

    static class IObjEtUtils
    {
        static internal Et SetOwnProperty(Et target, string name, Et value)
        {
            return Et.Call(
                target,
                typeof(IObj).GetMethod("SetOwnProperty"),
                Et.Constant(name),
                value
            );
        }
    }
}

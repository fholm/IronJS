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
}

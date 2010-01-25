using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Scripting.Utils;

using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Restrict = System.Dynamic.BindingRestrictions;
using EtParam = System.Linq.Expressions.ParameterExpression;

namespace IronJS.Runtime.Utils
{
    static class BinderUtils
    {
        static public bool NeedsToDefer(Meta target, Meta[] args, out Meta[] result)
        {
            if (!target.HasValue)
                goto Defer;

            for (int i = 0; i < args.Length; ++i)
            {
                if (!args[i].HasValue)
                    goto Defer;
            }

            result = null;
            return false;

            Defer:
                result = ArrayUtils.Insert(target, args);
                return true;
        }
    }
}

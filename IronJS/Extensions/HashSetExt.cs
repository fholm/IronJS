using System.Collections.Generic;
using IronJS.Compiler.Ast;

namespace IronJS.Extensions
{
    public static class HashSetExt
    {
        public static JsType EvalType(this HashSet<JsType> set)
        {
            set.Remove(JsType.Self);

            if (set.Count == 1)
            {
                if (set.Contains(JsType.Boolean))
                    return JsType.Boolean;

                if (set.Contains(JsType.Double))
                    return JsType.Double;

                if (set.Contains(JsType.String))
                    return JsType.String;

                if (set.Contains(JsType.Integer))
                    return JsType.Integer;

                if (set.Contains(JsType.Null))
                    return JsType.Null;

                if (set.Contains(JsType.Object))
                    return JsType.Object;

                if (set.Contains(JsType.Undefined))
                    return JsType.Undefined;
            }

            return JsType.Dynamic;
        }
    }
}

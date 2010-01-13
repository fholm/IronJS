using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Dynamic;
using System.Linq.Expressions;
using IronJS.Runtime;
using IronJS.Compiler.Ast;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;

namespace IronJS.Runtime.Binders
{
    using Et = System.Linq.Expressions.Expression;
    using ParamEt = System.Linq.Expressions.ParameterExpression;
    using Meta = System.Dynamic.DynamicMetaObject;
    using Restrict = System.Dynamic.BindingRestrictions;

    class JsSetIndexBinder : SetIndexBinder
    {
        Context _context;

        public JsSetIndexBinder(CallInfo callInfo, Context context)
            : base(callInfo)
        {
            _context = context;
        }

        public override Meta FallbackSetIndex(Meta target, Meta[] indexes, Meta value, Meta error)
        {
            throw new NotImplementedException();
        }
    }
}

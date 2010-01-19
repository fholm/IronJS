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

    class JsDeleteMemberBinder : DeleteMemberBinder
    {
        Context _context;

        public JsDeleteMemberBinder(object name, Context context)
            : base(name.ToString(), false)
        {
            _context = context;
        }

        public override Meta FallbackDeleteMember(Meta target, Meta errorSuggestion)
        {
            throw new NotImplementedException();
        }
    }
}

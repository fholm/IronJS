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

    class JsConvertBinder : ConvertBinder
    {
        public JsConvertBinder(Type type)
            : base(type, false)
        {

        }

        public override Meta FallbackConvert(Meta target, Meta errorSuggestion)
        {
            var restrictions = 
                Restrict.GetTypeRestriction(
                    target.Expression,
                    target.LimitType
                );

            if (Type == typeof(bool))
            {
                return new Meta(
                    TypeConverter.ToBoolean(target),
                    restrictions
                );
            }

            return EtUtils.CreateThrow(
                target,
                new Meta[] {},
                restrictions,
                typeof(Compiler.CompilerError),
                "No conversions for type '" + Type.Name + "' available"
            );
        }
    }
}

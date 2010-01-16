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
        Context _context;

        public JsConvertBinder(Type type, Context context)
            : base(type, false)
        {
            _context = context;
        }

        public override Meta FallbackConvert(Meta target, Meta errorSuggestion)
        {
            //TODO: insert defer

            var restrictions = 
                Restrict.GetTypeRestriction(
                    target.Expression,
                    target.LimitType
                );

            if (Type == typeof(Js.IObj))
                return new Meta(
                    EtUtils.Cast<IObj>(
                        TypeConverter.ToObject(target, _context)
                    ),
                    restrictions
                );

            if (Type == typeof(bool))
                return new Meta(
                    TypeConverter.ToBoolean(target),
                    restrictions
                );

            if (Type == typeof(double))
                return new Meta(
                    TypeConverter.ToNumber(target),
                    restrictions
                );

            if (Type == typeof(string))
                return new Meta(
                    TypeConverter.ToString(target),
                    restrictions
                );

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

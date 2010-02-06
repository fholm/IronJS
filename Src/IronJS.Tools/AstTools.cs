using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

#if CLR2
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Tools {

    #region Aliases
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = Expression;
    using EtParam = ParameterExpression;
    #endregion

    public static class AstTools {

        public static bool IsStrongBox(Et target) {
            return target.Type.IsGenericType && target.Type.GetGenericTypeDefinition() == typeof(StrongBox<>);
        }

        public static Et Value(EtParam target) {
            if (IsStrongBox(target))
                return Et.Field(target, "Value");

            return target;
        }

        public static Et Assign(EtParam target, Et value) {
            return Assign(target, value, true);
        }

        public static Et Assign(EtParam target, Et value, bool convert) {
            if (target.Type == value.Type)
                return Et.Assign(target, value);

            if (IsStrongBox(target))
                return AssignStrongBox(target, value, convert);

            if (!convert)
                throw new ArgumentException("Expression types do not match and conversion wasn't allowed");

            return AssignParameter(target, value);

        }

        #region Assign Helpers

        static Et AssignParameter(EtParam target, Et value) {
            if (target.Type.IsGenericType && value.Type.IsGenericType)
                throw new ArgumentException("Can't cast generic types");

            if (value.Type.IsValueType)
                Et.Assign(target, Et.Unbox(value, target.Type));

            return Et.Assign(target, Et.Convert(value, target.Type));
        }

        static Et AssignStrongBox(EtParam target, Et value, bool convert) {
            Type genericArg = target.Type.GetGenericArguments()[0];

            if (genericArg == value.Type || genericArg == typeof(object))
                return Et.Assign(Et.Field(target, "Value"), value);

            if (!convert)
                throw new ArgumentException("Expression types do not match and conversion wasn't allowed");

            if (value.Type.IsValueType)
                return Et.Assign(Et.Field(target, "Value"), Et.Unbox(value, genericArg));

            return Et.Assign(Et.Field(target, "Value"), Et.Convert(value, genericArg));
        }

        #endregion

        public static Et Box(Et value) {
            if (value.Type == typeof(void))
                return Et.Block(value, Et.Default(typeof(object)));

            return Et.Convert(value, typeof(object));
        }

        public static Et BuildBlock<T>(IEnumerable<T> collection, Func<T, Et> transform) {
            Et[] expressions = IEnumerableTools.Map(collection, transform);

            if (expressions.Length == 0)
                return AstUtils.Empty();

            return Et.Block(expressions);
        }

        public static Et Constant<T>(T value) {
            return Et.Constant(value, typeof(T));
        }

        public static Et New(Type type, params Et[] parameters) {
            ConstructorInfo ctor = type.GetConstructor(
                ArrayTools.Map(parameters, delegate(Et expr) {
                    return expr.Type;
                })
            );

            if (ctor == null)
                throw new NotImplementedException("No constructor taking these parameters exist");

            return AstUtils.SimpleNewHelper(ctor, parameters);
        }

        public static Et Debug(string msg) {
#if DEBUG
            return Et.Call(
                typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }),
                Constant(msg)
            );
#else		
			return AstUtils.Empty();
#endif
        }
    }
}

using System;
using System.Dynamic;
using System.Collections.Generic;
using System.Reflection;

#if CLR2
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Tools
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = Expression;

    public static class AstTools
    {
		public static Et Box(Et value)
		{
			if (value.Type == typeof(void))
				return Et.Block(
					value,
					Et.Default(typeof(object))
				);

			return Et.Convert(value, typeof(object));
		}

        public static Et BuildBlock<T>(IEnumerable<T> collection, Func<T, Et> filter)
        {
            Et[] expressions = IEnumerableTools.Map(collection, filter);

            if (expressions.Length == 0)
                return AstUtils.Empty();

            return Et.Block(expressions);
		}

		public static Et Constant<T>(T value)
		{
			return Et.Constant(value, typeof(T));
		}

		public static Et New(Type type, params Et[] parameters)
		{
			ConstructorInfo ctor = type.GetConstructor(
				ArrayTools.Map(parameters, delegate(Et expr)
			{
				return expr.Type;
			})
			);

			if (ctor == null)
				throw new NotImplementedException("No constructor taking these parameters exist");

			return AstUtils.SimpleNewHelper(ctor, parameters);
		}


		public static Et Debug(string msg)
		{
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

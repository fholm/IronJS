using System;
using System.Collections.Generic;
using Microsoft.Scripting.Utils;
using MetaObj = System.Dynamic.DynamicMetaObject;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Tools
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = Expression;

    public static class ArrayTools
    {
        public static T[] Filter<T>(T[] that, Func<T, bool> predicate)
        {
            List<T> list = new List<T>();

            for (int i = 0; i < that.Length; ++i)
                if (predicate(that[i]))
                    list.Add(that[i]);

            return list.ToArray();
        }

        public static T Last<T>(T[] that)
        {
            if (that.Length == 0)
                throw new ArgumentException("Array is empty");

            return that[that.Length - 1];
        }

        public static T First<T>(T[] that)
        {
            if (that.Length == 0)
                throw new ArgumentException("Array is empty");

            return that[0];
        }

        public static O[] Map<T, O>(T[] that, Func<T, O> func)
        {
            O[] array = new O[that.Length];

            for (int i = 0; i < that.Length; ++i)
                array[i] = func(that[i]);

            return array;
        }

        public static T[] Concat<T>(T[] a, T[] b)
        {
            T[] array = new T[a.Length + b.Length];

            Array.Copy(a, array, a.Length);
            Array.Copy(b, 0, array, a.Length, b.Length);

            return array;
        }

        public static Et ToBlock<T>(T[] that, Func<T, Et> transform)
        {
            if (that.Length == 0)
                return AstUtils.Empty();

            return Et.Block(Map(that, transform));
        }

        public static Type[] GetTypes(object[] that)
        {
            Type[] array = new Type[that.Length];

            for (int i = 0; i < that.Length; ++i)
                array[i] = that[i].GetType();

            return array;
        }

        public static T[] AddFirstAndLast<T>(T[] that, T first, T last)
        {
            return ArrayUtils.Insert(
                first,
                ArrayUtils.Append(
                    that, last
                )
            );
        }

        public static T[] DropFirstAndLast<T>(T[] that)
        {
            return ArrayUtils.RemoveFirst(
                ArrayUtils.RemoveLast(that)
            );
        }

        public static Type[] GetLimitTypes(MetaObj[] that)
        {
            Type[] array = new Type[that.Length];

            for (int i = 0; i < that.Length; ++i)
                array[i] = that[i].LimitType;

            return array;
        }

        public static Type[] GetExpressionTypes(MetaObj[] that)
        {
            Type[] array = new Type[that.Length];

            for (int i = 0; i < that.Length; ++i)
                array[i] = that[i].Expression.Type;

            return array;
        }
    }
}

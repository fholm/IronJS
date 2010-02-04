using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Scripting.Utils;
using MetaObj = System.Dynamic.DynamicMetaObject;

namespace IronJS.Extensions
{
    public static class ArrayExt
    {
        public static Type[] GetTypes(this object[] that)
        {
            return that.Select(x => x.GetType()).ToArray();
        }

        public static Type[] AddFirstAndLast(this Type[] that, Type first, Type last)
        {
            return ArrayUtils.Insert(
                first,
                ArrayUtils.Append(
                    that, last
                )
            );
        }

        public static Type[] DropFirstAndLast(this Type[] that)
        {
            return ArrayUtils.RemoveFirst(
                    ArrayUtils.RemoveLast(that)
                );
        }

        public static Type[] GetLimitTypes(this MetaObj[] that)
        {
            return that.Select(x => x.LimitType).ToArray();
        }

        public static Type[] GetExpressionTypes(this MetaObj[] that)
        {
            return that.Select(x => x.Expression.Type).ToArray();
        }
    }
}

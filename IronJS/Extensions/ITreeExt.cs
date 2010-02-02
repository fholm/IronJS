using System;
using System.Collections.Generic;
using Antlr.Runtime.Tree;

namespace IronJS.Extensions
{
    static class ITreeExt
    {
        public static ITree GetChildSafe(this ITree that, int n)
        {
            var child = that.GetChild(n);

            if (child == null)
                throw new Compiler.AstCompilerError("Expected child");

            if (!child.IsNil && child.Type == 0)
                throw new Compiler.AstCompilerError(String.Format("Unexpected '{0}'", child.Text));

            return child;
        }

        public static void EachChild(this ITree that, Action<ITree> act)
        {
            for(int i = 0; i < that.ChildCount; ++i)
            {
                act(that.GetChild(i));
            }
        }

        public static List<T> Map<T>(this ITree that, Func<ITree, T> act)
        {
            var list = new List<T>();

            for (int i = 0; i < that.ChildCount; ++i)
            {
                list.Add(act(that.GetChild(i)));
            }

            return list;
        }
    }
}

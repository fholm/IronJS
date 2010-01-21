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
                throw new Compiler.CompilerError("Expected child");

            if (!child.IsNil && child.Type == 0)
                throw new Compiler.CompilerError(String.Format("Unexpected '{0}'", child.Text));

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

        public static void Print(this ITree that, int indent = 0, int indentWidth = 2)
        {
            var indentStr = new String(' ', indent * indentWidth);

            Console.Write(indentStr + "(" + that.Text);

            if (that.ChildCount > 0)
            {
                Console.WriteLine("");
                that.EachChild(x => x.Print(indent + 2, indentWidth));
                Console.WriteLine(indentStr + ")");
            }
            else
            {
                Console.WriteLine(")");
            }
        }
    }
}

using System;
using IronJS.Ast.Nodes;
using IronJS.Runtime.Js;
using Microsoft.Scripting.Utils;
using System.Collections.Generic;

namespace IronJS.Tools
{
    static class HashSetTools
    {
        public static Type EvalType(HashSet<Type> set)
        {
            set.Remove(IjsTypes.Self);

            if (set.Count == 1)
                foreach (Type type in set)
                    return type;

            return typeof(object);
        }

        public static Type EvalType(HashSet<INode> set)
        {
            return HashSetTools.EvalType(
                new HashSet<Type>(
                    IEnumerableTools.Map(set, delegate(INode node) { return node.Type; })
                )
            );
        }
    }
}

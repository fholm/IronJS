using System;
using System.Collections.Generic;
using System.Linq;
using IronJS.Compiler.Ast;
using IronJS.Extensions;

namespace IronJS.Compiler.Optimizer
{
    public class IjsVarInfo
    {
        public string Name { get; set; }

        public bool IsResolving { get; set; }
        public bool IsParameter { get; set; }
        public bool IsDeletable { get; set; }
        public bool IsClosedOver { get; set; }

        public bool IsGlobal { get; set; }
        public bool IsLocal { get { return !IsGlobal; } }
        public bool IsAssignedOnce { get { return AssignedFrom.Count == 1; } }

        public Type ForcedType { protected get; set; }
        public HashSet<Type> UsedAs { get; protected set; }
        public HashSet<Ast.INode> AssignedFrom { get; protected set; }

        public Type ExprType
        {
            get
            {
                if (ForcedType != null)
                    return ForcedType;

                if (!IsResolving)
                {
                    IsResolving = true;

                    foreach (var node in AssignedFrom)
                        UsedAs.Add(node.ExprType);

                    IsResolving = false;
                    return UsedAs.EvalType();
                }

                // null is used to break circular references
                return null;
            }
        }

        public IjsVarInfo(string name)
        {
            Name = name;
            IsGlobal = false;
            IsParameter = false;
            IsResolving = false;
            IsDeletable = false;
            IsClosedOver = false;

            UsedAs = new HashSet<Type>();
            AssignedFrom = new HashSet<Ast.INode>();
        }
    }
}

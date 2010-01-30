using System;
using IronJS.Extensions;
using System.Collections.Generic;

namespace IronJS.Compiler.Optimizer
{
    public class Variable
    {
        public bool IsInsideWith { get; set; }
        public HashSet<Type> UsedAs { get; protected set; }
        public HashSet<Ast.INode> AssignedFrom { get; protected set; }
        public bool IsResolving { get; set; }
        public bool TypeResolved { get; set; }
        public bool CanBeDeleted { get; set; }

        public Type ExprType
        {
            get
            {
                if (!IsResolving)
                {
                    if(!TypeResolved)
                    {
                        IsResolving = true;

                        foreach (var node in AssignedFrom)
                            UsedAs.Add(node.ExprType);

                        TypeResolved = true;
                        IsResolving = false;
                    }

                    return UsedAs.EvalType();
                }

                return null;
            }
        }

        public Variable()
        {
            TypeResolved = false;
            IsResolving = false;
            IsInsideWith = false;
            UsedAs = new HashSet<Type>();
            AssignedFrom = new HashSet<Ast.INode>();
        }
    }
}

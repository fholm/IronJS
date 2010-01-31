using System;
using IronJS.Extensions;
using System.Collections.Generic;

namespace IronJS.Compiler.Optimizer
{
    public class Variable
    {

        public string Name { get; set; }
        public Ast.LambdaNode Lambda { get; set; }
        public bool IsResolving { get; set; }
        public bool IsParameter { get; set; }
        public bool IsInsideWith { get; set; }
        public bool TypeResolved { get; set; }
        public bool CanBeDeleted { get; set; }
        public bool IsClosedOver { get; set; }

        public HashSet<Type> UsedAs { get; protected set; }
        public HashSet<Ast.INode> AssignedFrom { get; protected set; }

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

        public Variable(string name)
        {
            Name = name;
            Lambda = null;
            IsParameter = false;
            IsResolving = false;
            TypeResolved = false;
            IsClosedOver = false;
            IsInsideWith = false;
            CanBeDeleted = false;

            UsedAs = new HashSet<Type>();
            AssignedFrom = new HashSet<Ast.INode>();
        }
    }
}

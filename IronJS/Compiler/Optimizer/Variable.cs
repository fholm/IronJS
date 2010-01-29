using System;
using IronJS.Extensions;
using System.Collections.Generic;

namespace IronJS.Compiler.Optimizer
{
    public class Variable
    {
        public bool IsInsideWith { get; set; }
        public HashSet<Ast.JsType> UsedAs { get; protected set; }
        public HashSet<Ast.INode> AssignedFrom { get; protected set; }
        public bool IsSearched { get; set; }
        public bool CanBeDeleted { get; set; }

        public Ast.JsType ExprType
        {
            get
            {
                if (!IsSearched)
                {
                    if(AssignedFrom != null)
                    {
                        IsSearched = true;
                        foreach (var node in AssignedFrom)
                            UsedAs.Add(node.ExprType);

                        AssignedFrom = null;
                        IsSearched = false;
                    }

                    return UsedAs.EvalType();
                }

                return Ast.JsType.Self;
            }
        }

        public Variable()
        {
            IsSearched = false;
            IsInsideWith = false;
            UsedAs = new HashSet<Ast.JsType>();
            AssignedFrom = new HashSet<Ast.INode>();
        }
    }
}

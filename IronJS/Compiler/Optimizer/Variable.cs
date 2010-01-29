using System;
using System.Collections.Generic;

namespace IronJS.Compiler.Optimizer
{
    public class Variable
    {
        public bool IsInsideWith { get; set; }
        public HashSet<Ast.JsType> UsedAs { get; protected set; }
        public HashSet<Ast.INode> AssignedFrom { get; protected set; }
        public Ast.JsType ExprType { get { return Ast.JsType.Dynamic; } }

        public Variable()
        {
            IsInsideWith = false;
            UsedAs = new HashSet<Ast.JsType>();
            AssignedFrom = new HashSet<Ast.INode>();
        }
    }
}

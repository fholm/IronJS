using System;
using System.Collections.Generic;

namespace IronJS.Compiler.Optimizer
{
    public class Variable
    {
        public bool IsInsideWith { get; set; }
        public HashSet<Ast.INode> AssignedFrom { get; protected set; }
        public Ast.JsType TypeOf { get { return Ast.JsType.Dynamic; } }

        public Variable()
        {
            IsInsideWith = false;
            AssignedFrom = new HashSet<Ast.INode>();
        }
    }
}

using System;
using System.Collections.Generic;

namespace IronJS.Compiler.Optimizer
{
    public enum VarType { String, Integer, Double, Boolean, Object, Null, Dynamic }

    public class Variable
    {
        public bool IsInsideWith { get; set; }
        public HashSet<Type> UsedWith { get; protected set; }

        public Variable()
        {
            IsInsideWith = false;
            UsedWith = new HashSet<Type>();
        }

        public VarType CalculateType()
        {
            if (UsedWith.Count == 0)
                throw new NotImplementedException();

            if (UsedWith.Count > 1)
            {
                if (UsedWith.Count == 2
                    && UsedWith.Contains(typeof(Ast.NumberNode<int>))
                    && UsedWith.Contains(typeof(Ast.NumberNode<double>)))
                {
                    return VarType.Double;
                }

                return VarType.Dynamic;
            }

            if (UsedWith.Contains(typeof(Ast.NumberNode<int>)))
                return VarType.Integer;

            if (UsedWith.Contains(typeof(Ast.NumberNode<double>)))
                return VarType.Double;

            if (UsedWith.Contains(typeof(Ast.StringNode)))
                return VarType.String;

            if (UsedWith.Contains(typeof(Ast.BooleanNode)))
                return VarType.Boolean;

            return VarType.Dynamic;
        }
    }
}

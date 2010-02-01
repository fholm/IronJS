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

        public IjsVarInfo(string name)
        {
            Name = name;
            IsParameter = false;
            IsResolving = false;
            TypeResolved = false;
            IsClosedOver = false;
            CanBeDeleted = false;

            UsedAs = new HashSet<Type>();
            AssignedFrom = new HashSet<Ast.INode>();
        }

        public bool GetFuncInfo(out IjsFuncInfo funcInfo)
        {
            if(ExprType == typeof(IjsFunc))
            {
                if(AssignedFrom.Count == 1)
                {
                    var first = AssignedFrom.First() as FuncNode;

                    if (first != null)
                    {
                        funcInfo = first.FuncInfo;
                        return true;
                    }
                }
            }

            funcInfo = null;
            return false;
        }
    }
}

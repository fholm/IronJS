using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using IronJS.Extensions;

namespace IronJS.Compiler
{
    public class IjsLocalVar : IjsCloseableVar
    {
        bool _resolvingType;

        public HashSet<Type> UsedAs { get; protected set; }
        public HashSet<Ast.INode> AssignedFrom { get; protected set; }
        public HashSet<IjsIVar> CopiedFrom { get; protected set; }

        public IjsLocalVar()
        {
            UsedAs = new HashSet<Type>();
            AssignedFrom = new HashSet<Ast.INode>();
            CopiedFrom = new HashSet<IjsIVar>();
        }

        #region IjsCloseableVar Members

        public bool IsClosedOver { get; set; }
        public ParameterExpression Expr { get; set; }

        public Type ExprType
        {
            get
            {
                if (!_resolvingType)
                {
                    _resolvingType = true;
                    var set = new HashSet<Type>(UsedAs);

                    foreach (var node in AssignedFrom)
                        set.Add(node.ExprType);

                    foreach (var var in CopiedFrom)
                        set.Add(var.ExprType);

                    _resolvingType = false;
                    return set.EvalType();
                }

                // null is used to break circular references
                return null;
            }
        }

        #endregion
    }
}

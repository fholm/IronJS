using System;
using System.Collections.Generic;
using IronJS.Compiler.Ast;
using IronJS.Runtime2.Js;
using IronJS.Tools;

#if CLR2
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;
using System.Runtime.CompilerServices;
#else
using System.Linq.Expressions;
#endif

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
                    HashSet<Type> set = new HashSet<Type>();
                    set.UnionWith(UsedAs);

                    foreach (INode node in AssignedFrom)
                        set.Add(node.ExprType);

                    foreach (IjsIVar variable in CopiedFrom)
                        set.Add(variable.ExprType);

                    _resolvingType = false;

					Type type = HashSetTools.EvalType(set);

					if (IsClosedOver)
						return typeof(StrongBox<>).MakeGenericType(type);

                    return type;
                }

                // null is used to break circular references
                return IjsTypes.Self;
            }
        }

        #endregion
    }
}

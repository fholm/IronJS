using System;
using IronJS.Tools;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;

namespace IronJS.Ast.Nodes {
    using Et = Expression;
    using EtParam = ParameterExpression;

    public class Local : Base, IVariable {
        protected EtParam Expr;
        protected readonly HashSet<Type> UsedAsSet;
        protected readonly HashSet<INode> UsedWithSet;

        public bool IsClosedOver {
            get; private set;
        }

        public Local(string name, NodeType nodeType)
            : base(nodeType) {
            Name = name;
            Expr = null;
            UsedAsSet = new HashSet<Type>();
            UsedWithSet = new HashSet<INode>();
        }

        public void MarkAsClosedOver() {
            IsClosedOver = true;
        }

        public override string ToString() {
            return base.ToString() + " " + Name + " <" + TypeTools.ShortName(Type) + ">";
        }

        #region Base Overrides

        public override Type Type {
            get {
                return HashSetTools.EvalType(UsedAsSet, UsedWithSet);
            }
        }

        public override Expression Compile(Lambda func) {
            return Expr;
        }

        #endregion

        #region IVariable Members

        public string Name {
            get; private set;
        }

        public void UsedAs(Type type) {
            UsedAsSet.Add(type);
        }

        public void UsedWith(INode node) {
            UsedWithSet.Add(node);
        }

        public void Clear() {
            Expr = null;
        }

        public void Setup() {
            if (IsClosedOver) {
                Expr = Et.Parameter(TypeTools.StrongBoxType.MakeGenericType(Type), Name);
            } else {
                Expr = Et.Parameter(Type, Name);
            }
        }

        #endregion
    }
}

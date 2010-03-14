using System;
using IronJS.Tools;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;

namespace IronJS.Ast.Nodes {
    using Et = Expression;
    using EtParam = ParameterExpression;

    public abstract class Variable : Base, IVariable {
        protected EtParam Expr;
        protected readonly HashSet<Type> UsedAsSet;
        protected readonly HashSet<INode> UsedWithSet;

        public Variable(string name, NodeType type)
            : base(type) {
            Name = name;
            Expr = null;
            UsedAsSet = new HashSet<Type>();
            UsedWithSet = new HashSet<INode>();
        }

        #region Base Members

        public override Expression Compile(Lambda func) {
            return Expr;
        }

        #endregion

        #region IVariable Members

        public string Name {
            get;
            protected set;
        }

        public void UsedAs(Type type) {
            UsedAsSet.Add(type);
        }

        public void UsedWith(INode node) {
            UsedWithSet.Add(node);
        }

        public void Reset() {
            Expr = null;
        }

        public abstract void Setup();

        #endregion
    }
}

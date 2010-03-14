using System;
using IronJS.Tools;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;

namespace IronJS.Ast.Nodes {
    using Et = Expression;
    using EtParam = ParameterExpression;

    public class Local : Variable {
        public bool IsClosedOver {
            get; private set;
        }

        public Local(string name, NodeType nodeType)
            : base(name, nodeType) {
        }

        public void MarkAsClosedOver() {
            IsClosedOver = true;
        }

        #region Base Members

        public override Type Type {
            get {
                return HashSetTools.EvalType(UsedAsSet, UsedWithSet);
            }
        }

        #endregion

        #region Variable Members

        public override void Setup() {
            if (IsClosedOver) {
                Expr = Et.Parameter(TypeTools.StrongBoxType.MakeGenericType(Type), Name);
            } else {
                Expr = Et.Parameter(Type, Name);
            }
        }

        #endregion

        #region Object Members

        public override string ToString() {
            return base.ToString() + " " + Name + " " + (IsClosedOver ? "Box" : "") + "<" + TypeTools.ShortName(Type) + ">";
        }

        #endregion
    }
}

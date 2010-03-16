using System;
using IronJS.Tools;
using IronJS.Runtime.Js;
using Microsoft.Scripting.Ast;

namespace IronJS.Ast.Nodes {
    using Et = Expression;
    using EtParam = ParameterExpression;

    public class Enclosed : Variable {
		readonly int _localId;
		readonly Lambda _func;

        public Enclosed(Lambda func, string name)
            : base(name, NodeType.Enclosed) {
			_func = func;
			_localId = func.Scope.Enclosed.Count;
        }

        #region Variable Members

        public override void Setup() {
            throw new NotImplementedException();
        }

        #endregion

        #region Object Members

        public override string ToString() {
            return base.ToString() + " " + Name + " <" + TypeTools.ShortName(Type) + ">";
        }

        #endregion
    }
}

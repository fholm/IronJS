using System;
using IronJS.Runtime.Js;
using IronJS.Tools;

namespace IronJS.Ast.Nodes {
    public class Param : Local {
        public Type InType {
            get; set;
        }

        public Param(string name)
            : base(name, NodeType.Param) {

        }

        #region Local Members

        public override Type Type {
            get {
                if (UsedAsSet.Count == 0 && UsedWithSet.Count == 0)
                    return InType;

                return TypeTools.EvalType(base.Type, InType);
            }
        }

        #endregion
    }
}

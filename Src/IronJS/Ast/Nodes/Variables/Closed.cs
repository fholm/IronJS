using System;
using IronJS.Runtime.Js;

namespace IronJS.Ast.Nodes {
    public class Closed : Variable {
        Lambda _lambda;

        public Closed(Lambda lambda, string name)
            : base(name, NodeType.Closed) {
            _lambda = lambda;
        }

        protected override Type EvalType() {
            return Types.Undefined;
        }
    }
}

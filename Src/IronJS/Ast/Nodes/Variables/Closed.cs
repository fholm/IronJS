using System;
using IronJS.Runtime.Js;

namespace IronJS.Ast.Nodes {
    public class Closed : Variable {
        Lambda _function;

        public Closed(Lambda function, string name)
            : base(name, NodeType.Closed) {
            _function = function;
        }

        protected override Type EvalType() {
            return IjsTypes.Undefined;
        }
    }
}

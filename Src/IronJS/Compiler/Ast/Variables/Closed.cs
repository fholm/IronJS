using System;
using IronJS.Runtime2.Js;

namespace IronJS.Compiler.Ast {
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

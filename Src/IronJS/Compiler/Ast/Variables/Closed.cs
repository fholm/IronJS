using System;
using IronJS.Runtime2.Js;

namespace IronJS.Compiler.Ast {
    public class Closed : Variable {
        Function _function;

        public Closed(Function function, string name)
            : base(name, NodeType.Closed) {
            _function = function;
        }

        protected override Type EvalType() {
            return IjsTypes.Undefined;
        }
    }
}

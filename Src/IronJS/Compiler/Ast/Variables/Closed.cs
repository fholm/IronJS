using System;
using System.Collections.Generic;
using System.Text;
using IronJS.Tools;

namespace IronJS.Compiler.Ast {
    public class Closed : Variable {
        Function _function;
        public Closed(Function function, string name)
            : base(name, NodeType.Closed) {
            _function = function;
        }
    }
}

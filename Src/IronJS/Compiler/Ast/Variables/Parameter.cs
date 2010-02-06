using System;
using System.Collections.Generic;
using System.Text;

namespace IronJS.Compiler.Ast {
    public class Parameter : Node, IVariable {
        public Parameter()
            : base(NodeType.Parameter, null) {

        }
    }
}

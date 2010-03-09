using System;
using System.Collections.Generic;
using System.Text;
using IronJS.Tools;

namespace IronJS.Compiler.Ast {
    public class Parameter : Variable {
        public Parameter(string name)
            : base(name, NodeType.Param) {
        }
    }
}

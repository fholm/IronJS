using System;
using System.Collections.Generic;
using System.Text;
using IronJS.Tools;

namespace IronJS.Ast.Nodes {
    public class Parameter : Variable {
        public Parameter(string name)
            : base(name, NodeType.Param) {
        }
    }
}

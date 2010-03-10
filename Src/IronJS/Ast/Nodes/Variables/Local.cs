using System;
using System.Collections.Generic;
using System.Text;
using IronJS.Tools;

namespace IronJS.Ast.Nodes {
    public class Local : Variable {
        public Local(string name)
            : base(name, NodeType.Local) {
        }
    }
}

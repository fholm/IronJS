using System;
using System.Collections.Generic;
using System.Text;

namespace IronJS.Compiler.Ast {
    public class Global2 : Node {
        public string Name { get; protected set; }

        public Global2(string name) 
            : base(NodeType.Global, null) {
            Name = name;
        }
    }
}

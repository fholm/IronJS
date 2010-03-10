using System;
using System.Collections.Generic;
using System.Text;
using IronJS.Tools;

namespace IronJS.Ast.Nodes {
    public class Global : Base {
        public string Name { get; protected set; }

        public Global(string name) 
            : base(NodeType.Global, null) {
            Name = name;
        }

		public override string ToString() {
			return base.ToString() + " " + Name;
		}
    }
}

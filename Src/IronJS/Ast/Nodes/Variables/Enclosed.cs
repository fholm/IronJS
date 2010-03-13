using System;
using IronJS.Runtime.Js;

namespace IronJS.Ast.Nodes {
    public class Enclosed : Variable {
        Lambda _lambda;

        public Enclosed(Lambda lambda, string name)
            : base(name, NodeType.Closed) {
            _lambda = lambda;
            ForceType(Types.Dynamic);
        }
    }
}

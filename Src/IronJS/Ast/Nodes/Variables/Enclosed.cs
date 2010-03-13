using System;
using IronJS.Runtime.Js;

namespace IronJS.Ast.Nodes {
    public class Enclosed : Base, IVariable {

        public Enclosed(Lambda lambda, string name)
            : base(NodeType.Closed) {

        }

        #region IVariable Members

        public string Name {
            get { throw new NotImplementedException(); }
        }

        public void UsedAs(Type type) {
            throw new NotImplementedException();
        }

        public void UsedWith(INode node) {
            throw new NotImplementedException();
        }

        public void Setup() {
            throw new NotImplementedException();
        }

        public void Clear() {
            throw new NotImplementedException();
        }

        #endregion
    }
}

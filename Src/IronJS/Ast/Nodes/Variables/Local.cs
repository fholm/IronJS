using IronJS.Tools;
using Microsoft.Scripting.Ast;

namespace IronJS.Ast.Nodes {
    using Et = Expression;

    public class Local : Variable {
        public Local(string name, NodeType nodeType)
            : base(name, nodeType) {
        }

        bool _isClosedOver;
        public void MarkAsClosedOver() {
            _isClosedOver = true;
        }

        public virtual void Setup() {
            if (_isClosedOver) {
                Expr = Et.Parameter(TypeTools.StrongBoxType.MakeGenericType(Type), Name);
            } else {
                Expr = Et.Parameter(Type, Name);
            }
        }
    }
}

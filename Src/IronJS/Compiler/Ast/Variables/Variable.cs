using System;
using System.Text;
using IronJS.Runtime2.Js;
using IronJS.Tools;

namespace IronJS.Compiler.Ast {
    public class Variable : Node {
        public bool IsClosedOver { get; set; }
        public string Name { get; private set; }

        public sealed override Type Type {
            get {
                if (_forcedType != null)
                    return _forcedType;

                return EvalType();
            }
        }

        public Variable(string name, NodeType nodeType)
            : base(nodeType, null) {
            Name = name;
        }

        Type _forcedType;
        public void ForceType(Type type) {
            _forcedType = type;
        }

        public override void Write(StringBuilder writer, int depth) {
            writer.AppendLine(StringTools.Indent(depth * 2) + "(" + NodeType + " " + Name + " " + TypeTools.ShortName(Type) + ")");
        }

        protected virtual Type EvalType() {
            return IjsTypes.Dynamic;
        }
    }
}

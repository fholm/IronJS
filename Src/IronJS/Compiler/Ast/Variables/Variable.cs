using System;
using System.Text;
using IronJS.Runtime2.Js;
using IronJS.Tools;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;

namespace IronJS.Compiler.Ast {
    using Et = Expression;
    using AstUtils = Utils;

    public class Variable : Node {
        public string Name { get; private set; }
        public ParameterExpression Expr { get; protected set; }

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
            _usedAs = new HashSet<Type>();
            _assignedFrom = new HashSet<INode>();
        }

        public void ClearExpr() {
            Expr = null;
        }

        HashSet<Type> _usedAs;
        public void UsedAs(Type type) {
            _usedAs.Add(type);
        }

        HashSet<INode> _assignedFrom;
        public void AssignedFrom(INode node) {
            _assignedFrom.Add(node);
        }

        Type _forcedType;
        public void ForceType(Type type) {
            _forcedType = type;
        }

        bool _isClosedOver;
        public void MarkAsClosedOver() {
            _isClosedOver = true;
        }

        public virtual void Setup() {
            if (_isClosedOver) {
                Expr = Et.Parameter(TypeTools.StrongBoxType.MakeGenericType(Type), "__" + Name + "__");
            } else {
                Expr = Et.Parameter(Type, "__" + Name + "__");
            }
        }

        public virtual Et Value() {
            return Expr;
        }

        public override sealed Expression Compile(Function func) {
            return Value();
        }

        public override void Write(StringBuilder writer, int depth) {
            writer.AppendLine(StringTools.Indent(depth * 2) + "(" + NodeType + " " + Name + " " + TypeTools.ShortName(Type) + ")");
        }

        protected virtual Type EvalType() {
            HashSet<Type> set = new HashSet<Type>();

            set.UnionWith(_usedAs);
            set.Add(HashSetTools.EvalType(_assignedFrom));

            return HashSetTools.EvalType(set);
        }
    }
}

using System;
using IronJS.Runtime.Jit;
using IronJS.Tools;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;

namespace IronJS.Ast.Nodes {
    using Et = Expression;

	public abstract class Variable : Base {
        public string Name { get; private set; }

		ParameterExpression _expr;
        public ParameterExpression Expr {
			get {
				if (_expr == null)
					Setup();

				return _expr;
			}
			set {
				_expr = value;
			}
		}

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
                Expr = Et.Parameter(TypeTools.StrongBoxType.MakeGenericType(Type), Name);
            } else {
                Expr = Et.Parameter(Type, Name);
            }
        }

		public virtual void Clear() {
			Expr = null;
		}

        public override sealed Et Compile(Lambda func) {
			return Expr;
        }

		public override string ToString() {
			return base.ToString() + " " + Name + " <" + TypeTools.ShortName(Type) + ">";
		}

        protected virtual Type EvalType() {
            HashSet<Type> set = new HashSet<Type>();

            set.UnionWith(_usedAs);
            set.Add(HashSetTools.EvalType(_assignedFrom));

            return HashSetTools.EvalType(set);
        }
    }
}

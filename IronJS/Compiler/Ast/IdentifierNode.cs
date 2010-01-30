using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class IdentifierNode : Node
    {
        public string Name { get; protected set; }
        public Optimizer.Variable Variable { get; set; }

        public bool IsDefinition { get; set; }
        public bool IsGlobal { get { return Variable == null; } }
        public bool IsDeletable { get { return IsGlobal ? false : Variable.CanBeDeleted; } }

        public IdentifierNode(string name, ITree node)
            : base(NodeType.Identifier, node)
        {
            Name = name;
            IsDefinition = false;
        }

        public override Type ExprType
        {
            get
            {
                if (IsGlobal)
                    return JsTypes.Dynamic;

                return Variable.ExprType;
            }
        }

        public override Et Generate2(EtGenerator etgen)
        {
            if (IsGlobal)
            {
                return Et.Call(
                    etgen.GlobalScopeExpr,
                    typeof(JsObj).GetMethod("Get"),
                    Et.Constant(Name, typeof(object))
                );
            }
            else
                return etgen.LambdaScope[Name];
        }

        public override INode Optimize(AstOptimizer astopt)
        {
            if (!astopt.IsGlobal)
            {
                if (IsDefinition)
                {
                    Variable = astopt.Scope.CreateVariable(Name);
                    Variable.IsInsideWith = astopt.IsInsideWith;
                }
                else
                {
                    Optimizer.Variable variable;

                    if (astopt.Scope.GetVariable(Name, out variable))
                        Variable = variable;
                }
            }

            return this;
        }

        public override Et Generate(EtGenerator etgen)
        {
            return Et.Call(
                etgen.FunctionScope.ScopeExpr,
                Scope.MiPull,
                Et.Constant(Name, typeof(object))
            );
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);
            writer.AppendLine(indentStr + "(" + Name + (IsDeletable ? "?" : "") + " " + ExprType + ")");
        }
    }
}

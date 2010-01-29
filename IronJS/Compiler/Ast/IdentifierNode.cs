using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class IdentifierNode : Node
    {
        public bool IsDefinition { get; set; }
        public bool IsGlobal { get { return Variable == null; } }
        public Optimizer.Variable Variable { get; set; }
        public string Name { get; protected set; }


        public IdentifierNode(string name, ITree node)
            : base(NodeType.Identifier, node)
        {
            Name = name;
            IsDefinition = false;
        }

        public Optimizer.VarType GetVarType()
        {
            if (Variable == null)
                return Optimizer.VarType.Dynamic;

            return Variable.CalculateType();
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);
            writer.AppendLine(indentStr + "(" + Name + " " + GetVarType() + ")");
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
    }
}

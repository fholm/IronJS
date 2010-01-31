using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;
using EtParam = System.Linq.Expressions.ParameterExpression;
using IronJS.Compiler.Optimizer;

namespace IronJS.Compiler.Ast
{
    public class AssignNode : Node, INode
    {
        public INode Target { get; protected set; }
        public INode Value { get; protected set; }

        public AssignNode(INode target, INode value, ITree node)
            : base(NodeType.Assign, node)
        {
            Target = target;
            Value = value;
        }

        public override Et GenerateStatic(IjsEtGenerator etgen)
        {
            var idNode = Target as IdentifierNode;
            if (idNode != null)
            {
                if (idNode.IsGlobal)
                {
                    return Et.Call(
                        etgen.GlobalsExpr,
                        typeof(IjsObj).GetMethod("Set"),
                        etgen.Constant<string>(idNode.Name),
                        EtUtils.Box2(Value.GenerateStatic(etgen))
                    );
                }
                else
                {
                    var typesMatch = Target.ExprType == Value.ExprType;
                    Tuple<EtParam, Variable> variable;

                    if (idNode.IsDefinition)
                    {
                        if (typesMatch)
                            variable = etgen.DefineVar(idNode.Variable);
                        else
                            variable = etgen.DefineVar(idNode.Variable);
                    }
                    else
                        variable = etgen.Scope[idNode.Name];

                    if (!typesMatch)
                    {
                        if (variable.Item1.Type != typeof(object))
                        {
                            throw new ArgumentException("Expression types did not mach, but variable.Type is not typeof(object)");
                        }

                        return Et.Assign(
                            variable.Item1,
                            EtUtils.Box2(Value.GenerateStatic(etgen))
                        );
                    }

                    return Et.Assign(
                        variable.Item1,
                        Value.GenerateStatic(etgen)
                    );
                }
            }

            throw new NotImplementedException();
        }

        public override Et Generate(EtGenerator etgen)
        {
            return etgen.GenerateAssign(
                Target,
                Value.Generate(etgen)
            );
        }

        public override INode Analyze(AstAnalyzer astopt)
        {
            Target = Target.Analyze(astopt);
            Value = Value.Analyze(astopt);

            var idNode = (Target as IdentifierNode);
            if (idNode != null && !idNode.IsGlobal)
                idNode.Variable.AssignedFrom.Add(Value);

            return this;
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType + " ");

            Target.Print(writer, indent + 1);
            Value.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}

using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Compiler.Optimizer;
using IronJS.Runtime.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class ReturnNode : Node
    {
        public INode Value { get; protected set; }
        public IjsFuncInfo FuncInfo { get; protected set; }

        public ReturnNode(INode value, ITree node)
            : base(NodeType.Return, node)
        {
            Value = value;
        }

        public override INode Analyze(IjsAstAnalyzer astopt)
        {
            Value = Value.Analyze(astopt);
            FuncInfo = astopt.Scope.FuncInfo;
            FuncInfo.Returns.Add(Value.ExprType);
            return this;
        }

        public override Et EtGen(IjsEtGenerator etgen)
        {
            return Et.Return(
                etgen.Scope.ReturnLabel,
                Et.Convert(
                    Value.EtGen(etgen), 
                    etgen.Scope.ReturnLabel.Type
                ),
                etgen.Scope.ReturnLabel.Type
            );
        }

        public override Et Generate(EtGenerator etgen)
        {
            return Et.Return(
                etgen.FunctionScope.ReturnLabel, 
                EtUtils.Cast<object>(Value.Generate(etgen)),
                typeof(object)
            );
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType);

            if (Value != null)
                Value.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}

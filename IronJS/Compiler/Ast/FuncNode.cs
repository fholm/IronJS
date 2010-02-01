using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Compiler.Optimizer;
using IronJS.Runtime;
using IronJS.Runtime.Js;
using IronJS.Extensions;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class FuncNode : Node
    {
        public INode Body { get; protected set; }
        public IdentifierNode Name { get; protected set; }
        public List<IdentifierNode> Args { get; protected set; }
        public IjsFuncInfo FuncInfo { get; protected set; }

        public FuncNode(List<IdentifierNode> args, INode body, IdentifierNode name, ITree node)
            : base(NodeType.Func, node)
        {
            Args = args;
            Body = body;
            Name = name;

            FuncInfo = new IjsFuncInfo(this);
            FuncInfo.IsLambda = Name == null;

            if (Name != null)
                Name.IsDefinition = true;
        }

        public override Type ExprType
        {
            get
            {
                return FuncInfo.ExprType;
            }
        }

        public override INode Analyze(IjsAstAnalyzer analyzer)
        {
            if (!analyzer.InGlobalScope)
            {
                if (!FuncInfo.IsLambda)
                {
                    Name.Analyze(analyzer);
                    Name.VarInfo.AssignedFrom.Add(this);
                    Name.VarInfo.UsedAs.Add(ExprType);
                }
            }

            analyzer.EnterScope(FuncInfo);

            foreach (var arg in Args)
            {
                arg.Analyze(analyzer);
                arg.VarInfo.IsParameter = true;
            }

            Body = Body.Analyze(analyzer);

            analyzer.ExitScope();

            return this;
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);
            var indentStr2 = new String(' ', (indent + 1) * 2);
            var indentStr3 = new String(' ', (indent + 3) * 2);

            writer.AppendLine(indentStr 
                + "(" + NodeType 
                + (" " + Name + " ").TrimEnd() 
                + " " + FuncInfo.ReturnType.ShortName()
            );

            if (FuncInfo.ClosesOver.Count > 0)
            {
                writer.AppendLine(indentStr2 + "(Closure");

                foreach (var id in FuncInfo.ClosesOver)
                    writer.AppendLine(indentStr3 + "(" + id.Name + ")");

                writer.AppendLine(indentStr2 + ")");
            }

            writer.Append(indentStr2 + "(Args");

            foreach (var node in Args)
                writer.Append(" " + node);

            writer.AppendLine(")");
            Body.Print(writer, indent + 1);
            writer.AppendLine(indentStr + ")");
        }
    }
}

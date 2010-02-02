using System;
using System.Linq;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;
using IronJS.Extensions;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class IdentifierNode : Node
    {
        public bool IsParameter { get; set; }
        public bool IsDefinition { get; set; }
        public IjsVarInfo VarInfo { get; set; }
        public string Name { get; protected set; }

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
                return VarInfo.ExprType;
            }
        }

        public override INode Analyze(FuncNode func)
        {
            if (IsDefinition)
            {
                VarInfo = func.CreateLocal(Name);
                VarInfo.IsGlobal = func.IsGlobalScope;
            }
            else
            {
                if (func.HasLocal(Name))
                {
                    VarInfo = func.GetLocal(Name);
                }
                else
                {
                    VarInfo = func.GetNonLocal(Name);
                }
            }

            return this;
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);
            writer.AppendLine(indentStr +
                "(" +
                    (IsDefinition ? ">" : "") +
                    (VarInfo.IsGlobal ? "$" : "") +
                    Name + 
                    (VarInfo.IsDeletable ? "!" : "") + 
                    (VarInfo.IsClosedOver ? "^" : "") + 
                    " " + 
                    ExprType.ShortName() + 
                ")"
            );
        }
    }
}

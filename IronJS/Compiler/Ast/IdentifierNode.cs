using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Compiler.Utils;
using IronJS.Extensions;
using IronJS.Runtime2.Js;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class IdentifierNode : Node
    {
        public bool IsParameter { get; set; }
        public bool IsDefinition { get; set; }
        public IjsVarInfo VarInfo { get; set; }
        public string Name { get; protected set; }
        public override Type ExprType { get { return VarInfo.ExprType; } }

        public IdentifierNode(string name, ITree node)
            : base(NodeType.Identifier, node)
        {
            Name = name;
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
                VarInfo = func.HasLocal(Name)
                        ? func.GetLocal(Name)
                        : func.GetNonLocal(Name);
            }

            return this;
        }

        public override Et EtGen(FuncNode func)
        {
            if (VarInfo.IsGlobal)
            {
                return Et.Convert(
                    Et.Call(
                        func.GlobalField,
                        typeof(IjsObj).GetMethod("Get"),
                        IjsEtGenUtils.Constant(Name)
                    ),
                    ExprType
                );
            }
            else
            {
                throw new NotImplementedException();
            }
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

using System;
using System.Linq;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Compiler.Optimizer;
using IronJS.Runtime.Js;
using IronJS.Extensions;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class IdentifierNode : Node
    {
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
                VarInfo = func.CreateVariable(Name);
                VarInfo.IsGlobal = func.IsGlobalScope;
            }
            else
            {
                IjsVarInfo variable;

                if (func.Function.GetVariable(Name, out variable))
                {
                    VarInfo = variable;

                    if (VarInfo.IsLocal)
                    {
                        var scope = func.Function;

                        while(!scope.HasVariable(Name))
                        {
                            VarInfo.IsClosedOver = true;
                            scope.FuncInfo.ClosesOver.Add(VarInfo);
                            scope = scope.Parent;
                        }
                    }
                }
                else
                {
                    VarInfo = func.GlobalScope.CreateVariable(Name);
                    VarInfo.IsGlobal = true;
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

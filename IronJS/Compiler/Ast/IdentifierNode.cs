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
        public string Name { get; protected set; }
        public IjsVarInfo VarInfo { get; set; }

        public bool IsDefinition { get; set; }
        public bool IsLocal { get { return VarInfo.IsLocal; } }
        public bool IsGlobal { get { return VarInfo.IsGlobal; } }

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

        public override INode Analyze(IjsAstAnalyzer astopt)
        {
            if (astopt.IsInsideWith)
                throw new NotImplementedException();

            if (IsDefinition)
            {
                VarInfo = astopt.Scope.CreateVariable(Name);
                VarInfo.IsGlobal = astopt.InGlobalScope;
            }
            else
            {
                IjsVarInfo variable;

                if (astopt.Scope.GetVariable(Name, out variable))
                {
                    VarInfo = variable;

                    if (VarInfo.IsLocal)
                    {
                        var scope = astopt.Scope;
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
                    VarInfo = astopt.GlobalScope.CreateVariable(Name);
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

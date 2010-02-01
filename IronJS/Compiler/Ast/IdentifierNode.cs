using System;
using System.Linq;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Compiler.Optimizer;
using IronJS.Runtime.Js;
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
                        if (!astopt.Scope.HasVariable(Name))
                        {
                            VarInfo.IsClosedOver = true;
                            astopt.Scope.FuncInfo.ClosesOver.Add(this);
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

        public override Et EtGen(IjsEtGenerator etgen)
        {
            if (IsGlobal)
            {
                return Et.Call(
                    etgen.GlobalsExpr,
                    typeof(IjsObj).GetMethod("Get"),
                    etgen.Constant(Name)
                );
            }
            else
            {
                if (IsDefinition)
                {

                }
                else
                {
                    if (etgen.Scope.FuncInfo.ClosesOver.Contains(this))
                    {
                        return Et.Field(
                            Et.Field(
                                etgen.ClosureExpr,
                                etgen.Scope.FuncInfo.ClosureType.GetField(Name)
                            ),
                            typeof(IjsClosureCell<>).MakeGenericType(ExprType).GetField("Value")
                        );
                    }

                    return etgen.Scope[Name].Item1;
                }
            }

            throw new NotImplementedException();
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);
            writer.AppendLine(indentStr + 
                "(" +
                    (VarInfo.IsGlobal ? "#" : "") +
                    Name + 
                    (VarInfo.IsDeletable ? "!" : "") + 
                    (VarInfo.IsClosedOver ? "?" : "") + 
                    " " + 
                    ExprType.Name.Split('.').Last() + 
                ")"
            );
        }
    }
}

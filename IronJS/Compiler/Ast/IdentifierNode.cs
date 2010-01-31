using System;
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

        public bool IsLocal { get { return !IsGlobal; } }
        public bool IsGlobal { get { return VarInfo == null; } }
        public bool IsDeletable { get { return IsGlobal ? false : VarInfo.CanBeDeleted; } }

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
                    return IjsTypes.Dynamic;

                return VarInfo.ExprType;
            }
        }

        public override INode Analyze(IjsAstAnalyzer astopt)
        {
            if (!astopt.IsInsideWith)
            {
                if (!astopt.InGlobalScope)
                {
                    if (IsDefinition)
                    {
                        VarInfo = astopt.Scope.CreateVariable(Name);
                    }
                    else
                    {
                        IjsVarInfo variable;

                        if (astopt.Scope.GetVariable(Name, out variable))
                        {
                            VarInfo = variable;

                            if (!astopt.Scope.Variables.ContainsKey(Name))
                                astopt.Scope.FuncInfo.ClosesOver.Add(this);
                        }
                    }
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

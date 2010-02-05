using System;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Compiler.Tools;
using IronJS.Tools;
using IronJS.Runtime2.Js;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast
{
    using Et = Expression;

    public class IdentifierNode : Node
    {
        public bool IsDefinition { get; set; }
        public IjsIVar VarInfo { get; set; }
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
            }
            else
            {
                VarInfo = (func.HasLocal(Name) | func.HasParameter(Name))
                        ? func.GetLocal(Name)
                        : func.GetNonLocal(Name);
            }

            return this;
        }

        public override Et EtGen(FuncNode func)
        {
            if (func.IsGlobal(VarInfo))
            {
                IjsLocalVar varInfo = VarInfo as IjsLocalVar;

                if (varInfo.AssignedFrom.Count == 0)
                {
                    return Et.Call(
                        func.GlobalField,
                        typeof(IjsObj).GetMethod("Get"),
                        IjsEtGenUtils.Constant(Name)
                    );
                }
                else
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
            }
            else if (func.IsLocal(VarInfo))
            {
                return VarInfo.Expr;
            }
            else
            {
                throw new NotImplementedException();
            }
        }

        public override void Print(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);

            writer.AppendLine(
                indentStr + "(" + (IsDefinition ? ">" : "") + Name + " " + TypeTools.ShortName(ExprType) + ")"
            );
        }
    }
}

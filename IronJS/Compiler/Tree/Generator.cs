using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using Microsoft.Scripting.Utils;
using System.Linq.Expressions;
using IronJS.Extensions;
using IronJS.Runtime.Utils;
using IronJS.Runtime.Binders;
using IronJS.Runtime.Js;

namespace IronJS.Compiler.Tree
{
    using Et = System.Linq.Expressions.Expression;
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Js = IronJS.Runtime.Js;
    using Binders = IronJS.Runtime.Binders;
    using IronJS.Runtime;

    public class Generator
    {
        internal int FuncCounter;
        internal Frame<Function> Table;

        internal Stack<ParameterExpression> FrameExprStack;
        internal Stack<ParameterExpression> TableExprStack;

        internal ParameterExpression FrameExpr
        {
            get { return FrameExprStack.Peek(); }
        }

        internal ParameterExpression TableExpr
        {
            get { return TableExprStack.Peek(); }
        }

        public Action<Frame<Function>, Frame<object>> Build(List<Ast.Node> astNodes, out Frame<Function> funcTable)
        {
            FuncCounter = 0;

            FrameExprStack = new Stack<ParameterExpression>();
            TableExprStack = new Stack<ParameterExpression>();

            funcTable = Table = new Frame<Function>(null);

            EnterScope();

            var exprs = new List<Et>();

            foreach (var node in astNodes)
                exprs.Add(Generate(node));

            return Et.Lambda<Action<Frame<Function>, Frame<object>>>(
                Et.Block(exprs),
                TableExpr,
                FrameExpr
            ).Compile();
        }

        private string NewFuncName()
        {
            return "#function" + (FuncCounter++).ToString();
        }

        private void EnterScope()
        {
            TableExprStack.Push(Et.Parameter(typeof(Frame<Function>), "#table"));
            FrameExprStack.Push(Et.Parameter(typeof(Frame<object>), "#frame"));
        }

        private void ExitScope()
        {
            TableExprStack.Pop();
            FrameExprStack.Pop();
        }

        private Et Generate(Ast.Node node)
        {
            switch (node.Type)
            {
                case Ast.NodeType.Assign:
                    return GenerateAssign((Ast.AssignNode)node);

                case Ast.NodeType.Lambda:
                    return GenerateLambda((Ast.LambdaNode)node);

                case Ast.NodeType.Block:
                    return GenerateBlock((Ast.BlockNode)node);

                case Ast.NodeType.Call:
                    return GenerateCall((Ast.CallNode)node);

                case Ast.NodeType.Number:
                    return GenerateNumber((Ast.NumberNode)node);

                case Ast.NodeType.Identifier:
                    return GenerateIdentifier((Ast.IdentifierNode)node);

                case Ast.NodeType.BinaryOp:
                    return GenerateBinaryOp((Ast.BinaryOpNode)node);

                default:
                    throw new Compiler.CompilerError("Unsupported AST node '" + node.Type + "'");
            }
        }

        private Et GenerateBinaryOp(Ast.BinaryOpNode node)
        {
            switch (node.Op)
            {
                case Ast.BinaryOp.Add:
                    return Et.Add(Generate(node.Left), Generate(node.Right), typeof(BuiltIns).GetMethod("Add"));

                default:
                    throw new CompilerError("Unsuported binary op '" + node.Op + "'");
            }
        }

        private Et GenerateNumber(Ast.NumberNode node)
        {
            return Et.Constant(node.Value);
        }

        private Et GenerateIdentifier(Ast.IdentifierNode node)
        {
            return Frame<object>.Var(FrameExpr, node.Name);
        }

        private Et GenerateCall(Ast.CallNode node)
        {
            var target = Generate(node.Target);
            var args = node.Args.ToEtArray(x => Generate(x));

            return Et.Dynamic(
                new JsInvokeBinder(
                    new CallInfo(args.Length + 1), 
                    InvokeFlag.Function
                ),
                typeof(object),
                ArrayUtils.Insert(
                    target,
                    ArrayUtils.Insert(
                        TableExpr,
                        FrameExpr,
                        args
                    )
                )
            ); 
        }

        private Et GenerateBlock(Ast.BlockNode node)
        {
            return Et.Block(node.Nodes.Select(x => Generate(x)));
        }

        private Et GenerateLambda(Ast.LambdaNode node)
        {
            EnterScope();

            var funcName = NewFuncName();
            var argsList = node.Args.Select(x => x.Name).ToList();
            var bodyExpr = Generate(node.Body);
            var compiled = 
                Et.Lambda<Action<Frame<Function>, Frame<object>>>(
                    bodyExpr, 
                    TableExpr, 
                    FrameExpr
                ).Compile();

            Table.Push(funcName, new Function(compiled, argsList));

            ExitScope();

            return GenerateClosure(GetFunction(funcName));
        }

        private Et GenerateClosure(Et funcExpr)
        {
            return AstUtils.SimpleNewHelper(
                typeof(Closure).GetConstructor(new[] 
                { 
                    typeof(Frame<Function>), 
                    typeof(Frame<object>),
                    typeof(Function)
                }),
                TableExpr,
                FrameExpr,
                EtUtils.Cast<Function>(funcExpr)
            );
        }

        private Et GetFunction(string funcName)
        {
            return Et.Dynamic(
                new JsGetMemberBinder(funcName),
                typeof(object),
                TableExpr
            );
        }

        private Et GenerateAssign(Ast.AssignNode node)
        {
            if(node.Target is Ast.IdentifierNode)
            {
                var idNode = (Ast.IdentifierNode) node.Target;
                return Frame<object>.Var(FrameExpr, idNode.Name, Generate(node.Value));
            }

            throw new NotImplementedException();
        }
    }
}

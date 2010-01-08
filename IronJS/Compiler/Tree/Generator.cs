using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using IronJS.Extensions;
using IronJS.Runtime;
using IronJS.Runtime.Binders;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using IronJS.Reflect;
using Microsoft.Scripting.Utils;

namespace IronJS.Compiler.Tree
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = System.Linq.Expressions.Expression;

    public class Generator
    {
        internal Stack<ParameterExpression> FrameExprStack;
        internal List<Tuple<Et, List<string>>> LambdaExprs;
        internal ParameterExpression TableExpr;

        internal ParameterExpression FrameExpr
        {
            get { return FrameExprStack.Peek(); }
        }

        internal int LambdaId
        {
            get { return LambdaExprs.Count - 1; }
        }

        public Action<Frame> Build(List<Ast.Node> astNodes)
        {
            LambdaExprs = new List<Tuple<Et, List<string>>>();
            FrameExprStack = new Stack<ParameterExpression>();
            TableExpr = Et.Parameter(typeof(Table), "#functbl");

            EnterFrame();

            var globalExprs = new List<Et>();

            foreach (var node in astNodes)
            {
                globalExprs.Add(Generate(node));
            }

            var buildLambdaExprs = new List<Et>();

            buildLambdaExprs.Add(
                Et.Assign(
                    TableExpr,
                    AstUtils.SimpleNewHelper(
                        typeof(Table).GetConstructor(Type.EmptyTypes)
                    )
                )
            );

            //TODO: remove after debugging
            buildLambdaExprs.Add(
                Frame.Var(
                    FrameExpr, 
                    "functbl", 
                    TableExpr, 
                    VarType.Local
                )
            );

            var tablePushMi = typeof(Table).GetMethod("Push");
            var functionCtor = typeof(Function).GetConstructor(
                new[] { 
                    typeof(Action<Frame>), 
                    typeof(List<string>)
                }
            );

            foreach (var lambda in LambdaExprs)
            {
                buildLambdaExprs.Add(
                    Et.Call(
                        TableExpr,
                        tablePushMi,
                        AstUtils.SimpleNewHelper(functionCtor, lambda.V1, Et.Constant(lambda.V2))
                    )
                );
            }

            var allExprs = CollectionUtils.Concat(
                buildLambdaExprs, 
                globalExprs
            );

            return Et.Lambda<Action<Frame>>(
                Et.Block(
                    new[] { TableExpr },
                    allExprs
                ),
                FrameExpr
            ).Compile();
        }

        private void EnterFrame()
        {
            FrameExprStack.Push(Et.Parameter(typeof(Frame), "#frame"));
        }

        private void ExitFrame()
        {
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
            return Et.Add(
                EtUtils.Cast<double>(Generate(node.Left)),
                EtUtils.Cast<double>(Generate(node.Right)),
                typeof(BuiltIns).GetMethod("Add")
            );
        }

        private Et GenerateNumber(Ast.NumberNode node)
        {
            return Et.Constant(node.Value, typeof(object));
        }

        private Et GenerateIdentifier(Ast.IdentifierNode node)
        {
            return Frame.Var(FrameExpr, node.Name);
        }

        private Et GenerateCall(Ast.CallNode node)
        {
            var target = Generate(node.Target);
            var args = node.Args.ToEtArray(x => Generate(x));

            return Et.Dynamic(
                new JsInvokeBinder(
                    new CallInfo(args.Length),
                    InvokeFlag.Function
                ),
                typeof(object),
                ArrayUtils.Insert(
                    target,
                    FrameExpr,
                    args
                )
            );
        }

        private Et GenerateBlock(Ast.BlockNode node)
        {
            return Et.Block(node.Nodes.Select(x => Generate(x)));
        }

        private Et GenerateLambda(Ast.LambdaNode node)
        {
            EnterFrame();

            var argsList = node.Args.Select(x => x.Name).ToList();
            var bodyExpr = Generate(node.Body);
            var lambdaEt = Et.Lambda<Action<Frame>>(
                bodyExpr,
                FrameExpr
            );

            LambdaExprs.Add(
                Tuple.Create(
                    (Et) lambdaEt, 
                    argsList
                )
            );

            ExitFrame();

            return AstUtils.SimpleNewHelper(
                typeof(Closure).GetConstructor(new[] 
                { 
                    typeof(Frame), 
                    typeof(Function)
                }),
                FrameExpr,
                Et.Call(
                    TableExpr,
                    typeof(Table).GetMethod("Pull"),
                    Et.Constant(LambdaId)
                )
            );
        }

        private Et GenerateClosure(Et funcExpr)
        {
            throw new NotImplementedException();
        }

        private Et GetFunction(string funcName)
        {
            throw new NotImplementedException();
        }

        private Et GenerateAssign(Ast.AssignNode node)
        {
            if (node.Target is Ast.IdentifierNode)
            {
                var idNode = (Ast.IdentifierNode) node.Target;

                return Frame.Var(
                    FrameExpr, 
                    idNode.Name, 
                    Generate(node.Value), 
                    idNode.IsLocal ? VarType.Local : VarType.Global
                );
            }

            throw new NotImplementedException();
        }
    }
}

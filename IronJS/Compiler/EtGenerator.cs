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
using Microsoft.Scripting.Utils;

namespace IronJS.Compiler
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = System.Linq.Expressions.Expression;

    public class EtGenerator
    {
        internal ParameterExpression TableExpr;
        internal ParameterExpression GlobalFrameExpr;
        internal Stack<LabelTarget> ReturnLabels;
        internal Stack<ParameterExpression> FrameExprStack;
        internal List<Tuple<Et, List<string>>> LambdaExprs;
        internal List<Et> GlobalExprs;

        internal ParameterExpression FrameExpr
        {
            get { return FrameExprStack.Peek(); }
        }

        internal int LambdaId
        {
            get { return LambdaExprs.Count - 1; }
        }

        internal LabelTarget ReturnLabel
        {
            get { return ReturnLabels.Peek(); }   
        }

        public Action<Frame> Build(List<Ast.Node> astNodes)
        {
            GlobalExprs = new List<Et>();
            ReturnLabels = new Stack<LabelTarget>();
            LambdaExprs = new List<Tuple<Et, List<string>>>();
            FrameExprStack = new Stack<ParameterExpression>();
            TableExpr = Et.Parameter(typeof(Table), "#functbl");

            EnterFrame();

            GlobalFrameExpr = FrameExpr;

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
                FrameUtils.Push(
                    FrameExpr, 
                    "functbl", 
                    TableExpr, 
                    VarType.Local
                )
            );

            var tablePushMi = typeof(Table).GetMethod("Push");
            var functionCtor = typeof(Lambda).GetConstructor(
                new[] { 
                    typeof(Func<Frame, object>), 
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
            ReturnLabels.Push(Et.Label(typeof(object), "#return"));
            FrameExprStack.Push(Et.Parameter(typeof(Frame), "#frame"));
        }

        private void ExitFrame()
        {
            FrameExprStack.Pop();
            ReturnLabels.Pop();
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

                case Ast.NodeType.Identifier:
                    return GenerateIdentifier((Ast.IdentifierNode)node);

                case Ast.NodeType.BinaryOp:
                    return GenerateBinaryOp((Ast.BinaryOpNode)node);

                case Ast.NodeType.UnaryOp:
                    return GenerateUnaryOp((Ast.UnaryOpNode)node);

                case Ast.NodeType.Return:
                    return GenerateReturn((Ast.ReturnNode)node);

                case Ast.NodeType.New:
                    return GenerateNew((Ast.NewNode)node);

                case Ast.NodeType.MemberAccess:
                    return GenerateMemberAccess((Ast.MemberAccessNode)node);

                case Ast.NodeType.Logical:
                    return GenerateLogical((Ast.LogicalNode)node);

                case Ast.NodeType.PostfixOperator:
                    return GeneratePostFixOp((Ast.PostfixOperatorNode)node);

                case Ast.NodeType.TypeOf:
                    return GenerateTypeOf((Ast.TypeOfNode)node);

                case Ast.NodeType.Boolean:
                    return GenerateBoolean((Ast.BooleanNode)node);

                case Ast.NodeType.Void:
                    return GenerateVoid((Ast.VoidNode)node);

                #region Constants

                case Ast.NodeType.Number:
                    return GenerateNumber((Ast.NumberNode)node);

                case Ast.NodeType.String:
                    return GenerateString((Ast.StringNode)node);

                case Ast.NodeType.Null:
                    return GenerateNull((Ast.NullNode)node);

                #endregion

                default:
                    throw new Compiler.CompilerError("Unsupported AST node '" + node.Type + "'");
            }
        }

        private Et GenerateVoid(Ast.VoidNode node)
        {
            return Et.Block(
                Generate(node.Target),
                Undefined.Expr
            );
        }

        private Et GenerateBoolean(Ast.BooleanNode node)
        {
            return Et.Constant(
                node.Value, 
                typeof(object)
            );
        }

        private Et GenerateTypeOf(Ast.TypeOfNode node)
        {
            return Et.Call(
                typeof(BuiltIns).GetMethod("TypeOf"),
                Generate(node.Target)
            );
        }

        private Et GeneratePostFixOp(Ast.PostfixOperatorNode node)
        {
            var target = Generate(node.Target);
            var tmp = Et.Parameter(typeof(double), "#tmp");

            return Et.Block(
                new[] { tmp },

                // the value we will return
                Et.Assign(
                    tmp, 
                    Et.Dynamic(
                        new JsConvertBinder(typeof(double)),
                        typeof(double),
                        target
                    )
                ),

                // calc new value
                BuildAssign(
                    node.Target,
                    EtUtils.Box(
                        Et.Add(
                            tmp, 
                            Et.Constant(
                                node.Op == ExpressionType.PostIncrementAssign
                                         ? 1.0 
                                         : -1.0,
                                typeof(double)
                            )
                        )
                    )
                ),

                tmp // return the old value
            );

            throw new NotImplementedException();
        }

        private Et GenerateLogical(Ast.LogicalNode node)
        {
            var tmp = Et.Parameter(typeof(object), "#tmp");

            return Et.Block(
                new[] { tmp },
                Et.Assign(tmp, Generate(node.Left)),
                Et.Condition(
                    Et.Dynamic(
                        new JsConvertBinder(typeof(bool)),
                        typeof(bool),
                        tmp
                    ),
                    node.Op == ExpressionType.AndAlso 
                             ? Generate(node.Right)  // &&
                             : tmp,                  // ||
                    node.Op == ExpressionType.AndAlso 
                             ? tmp                   // &&
                             : Generate(node.Right)  // ||
                )
            );
        }

        private Et GenerateUnaryOp(Ast.UnaryOpNode node)
        {
            return Et.Dynamic(
                new JsUnaryOpBinder(node.Op),
                typeof(object),
                Generate(node.Target)
            );
        }

        private Et GenerateNull(Ast.NullNode node)
        {
            return Et.Default(typeof(object));
        }

        private Et GenerateMemberAccess(Ast.MemberAccessNode node)
        {
            return Et.Dynamic(
                new JsGetMemberBinder(node.Name),
                typeof(object),
                Generate(node.Target)
            );
        }

        private Et GenerateNew(Ast.NewNode node)
        {
            var target = Generate(node.Target);
            var args = node.Args.ToEtArray(x => Generate(x));
            var tmp = Et.Variable(typeof(Obj), "#tmp");
            var exprs = new List<Et>();

            exprs.Add(
                Et.Assign(
                    tmp,
                    EtUtils.Cast<Obj>(
                        Et.Dynamic(
                            new JsInvokeBinder(
                                new CallInfo(args.Length),
                                InvokeFlag.Constructor
                            ),
                            typeof(object),
                            ArrayUtils.Insert(
                                target,
                                args
                            )
                        )
                    )
                )
            );

            foreach(var propNode in node.Properties)
            {
                exprs.Add(
                    Et.Call(
                        tmp,
                        typeof(Obj).GetMethod("Put"),
                        Et.Constant(propNode.Name, typeof(object)),
                        EtUtils.Box(Generate(propNode.Value))
                    )
                );
            }

            exprs.Add(
                EtUtils.Box(tmp)
            );

            return Et.Block(
                new[] { tmp },
                exprs
            );
        }

        private Et GenerateReturn(Ast.ReturnNode node)
        {
            return Et.Return(ReturnLabel, Generate(node.Value), typeof(object));
        }

        private Et GenerateString(Ast.StringNode node)
        {
            return Et.Constant(node.Value, typeof(object));
        }

        private Et GenerateNumber(Ast.NumberNode node)
        {
            return Et.Constant(node.Value, typeof(object));
        }

        private Et GenerateIdentifier(Ast.IdentifierNode node)
        {
            return FrameUtils.Pull(FrameExpr, node.Name);
        }

        private Et GenerateBlock(Ast.BlockNode node)
        {
            if (node.Nodes.Count == 0)
                return Et.Default(typeof(object));

            return Et.Block(
                node.Nodes.Select(x => Generate(x))
            );
        }

        private Et GenerateBinaryOp(Ast.BinaryOpNode node)
        {
            return Et.Dynamic(
                new JsBinaryOpBinder(node.Op),
                typeof(object),
                Generate(node.Left),
                Generate(node.Right)
            );
        }

        private Et GenerateCall(Ast.CallNode node)
        {
            var args = node.Args.ToEtArray(x => Generate(x));

            if (node.Target is Ast.IdentifierNode)
            {
                var target = Generate(node.Target);

                return Et.Dynamic(
                    new JsInvokeBinder(
                        new CallInfo(args.Length),
                        InvokeFlag.Function
                    ),
                    typeof(object),
                    ArrayUtils.Insert(
                        target,
                        args
                    )
                );
            }
            else if(node.Target is Ast.MemberAccessNode)
            {
                var target = (Ast.MemberAccessNode)node.Target;

                return Et.Dynamic(
                    new JsInvokeMemberBinder(
                        target.Name,
                        new CallInfo(args.Length + 1)
                    ),
                    typeof(object),
                    ArrayUtils.Insert(
                        Generate(target.Target),
                        args
                    )
                );
            }

            throw new NotImplementedException();
        }

        private Et GenerateLambda(Ast.LambdaNode node)
        {
            EnterFrame();

            var bodyNode = (Ast.BlockNode)node.Body;
            var argsList = node.Args.Select(x => x.Name).ToList();
            var bodyExprs = bodyNode.Nodes.Select(x => Generate(x)).ToList();

            bodyExprs.Add(
                Et.Label(
                    ReturnLabel, 
                    Expression.Default(typeof(object))
                )
            );

            var lambdaEt = Et.Lambda<Func<Frame, object>>(
                Et.Block(bodyExprs),
                FrameExpr
            );

            LambdaExprs.Add(
                Tuple.Create(
                    (Et) lambdaEt, 
                    argsList
                )
            );

            ExitFrame();

            // temp storage for the new object
            var tmpObj = Et.Variable(
                typeof(Obj), 
                "#tmpobj"
            );

            /*
             * 1) Create a new object with the current frame and current lambda as params
             * 2) Assign the #FunctionPrototype object to it's Prototype field
             * 3) return it
             */

            return Et.Block(
                new[] { tmpObj },
                // 1
                Et.Assign(
                    tmpObj,
                    AstUtils.SimpleNewHelper( 
                        typeof(Obj).GetConstructor(
                            new[] { 
                                typeof(Frame), 
                                typeof(Lambda)
                            }
                        ),
                        FrameExpr,
                        Et.Call(
                            TableExpr,
                            typeof(Table).GetMethod("Pull"),
                            Et.Constant(LambdaId)
                        )
                    )
                ),
                // 2
                Et.Assign(
                    Et.MakeMemberAccess(
                        tmpObj,
                        typeof(Obj).GetField("Prototype")
                    ),
                    FrameUtils.Pull<Obj>(
                        GlobalFrameExpr,
                        "#FunctionPrototype"
                    )
                ),
                // 3
                tmpObj
            );
        }

        private Et GenerateAssign(Ast.AssignNode node)
        {
            return BuildAssign(node.Target, Generate(node.Value));
        }

        private Et BuildAssign(Ast.Node target, Et value)
        {
            if (target is Ast.IdentifierNode)
            {
                var idNode = (Ast.IdentifierNode)target;

                return FrameUtils.Push(
                    FrameExpr,
                    idNode.Name,
                    value,
                    idNode.IsLocal ? VarType.Local : VarType.Global
                );
            }
            else if (target is Ast.MemberAccessNode)
            {
                var maNode = (Ast.MemberAccessNode)target;

                return Et.Dynamic(
                    new JsSetMemberBinder(maNode.Name),
                    typeof(object),
                    Generate(maNode.Target),
                    value
                );
            }

            throw new NotImplementedException();
        }
    }
}

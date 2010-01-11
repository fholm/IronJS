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
        internal Stack<LabelTarget> BreakLabels;
        internal Stack<LabelTarget> ContinueLabels;
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

        internal LabelTarget BreakLabel
        {
            get { return BreakLabels.Peek(); }
        }

        internal LabelTarget ContinueLabel
        {
            get { return ContinueLabels.Peek(); }
        }

        public Action<IFrame> Build(List<Ast.Node> astNodes)
        {
            GlobalExprs = new List<Et>();
            ReturnLabels = new Stack<LabelTarget>();
            LambdaExprs = new List<Tuple<Et, List<string>>>();
            FrameExprStack = new Stack<ParameterExpression>();
            BreakLabels = new Stack<LabelTarget>();
            ContinueLabels = new Stack<LabelTarget>();
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
                    typeof(Func<IFrame, object>), 
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

            return Et.Lambda<Action<IFrame>>(
                Et.Block(
                    new[] { TableExpr },
                    allExprs
                ),
                FrameExpr
            ).Compile();
        }

        private void EnterLoop()
        {
            BreakLabels.Push(Et.Label(typeof(void), "#break"));
            ContinueLabels.Push(Et.Label(typeof(void), "#continue"));
        }

        private void ExitLoop()
        {
            ContinueLabels.Pop();
            BreakLabels.Pop();
        }

        private void EnterFrame()
        {
            ReturnLabels.Push(Et.Label(typeof(object), "#return"));
            FrameExprStack.Push(Et.Parameter(typeof(IFrame), "#frame"));
        }

        private void ExitFrame()
        {
            FrameExprStack.Pop();
            ReturnLabels.Pop();
        }

        private Et Generate(Ast.Node node)
        {
            if (node == null)
                return AstUtils.Empty();

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

                case Ast.NodeType.If:
                    return GenerateIf((Ast.IfNode)node);

                case Ast.NodeType.StrictCompare:
                    return GenerateStrictCompare((Ast.StrictCompareNode)node);

                case Ast.NodeType.UnsignedRightShift:
                    return GenerateUnsignedRightShift((Ast.UnsignedRightShiftNode)node);

                case Ast.NodeType.While:
                    return GenerateWhile((Ast.WhileNode)node);

                case Ast.NodeType.ForStep:
                    return GenerateForStep((Ast.ForStepNode)node);

                case Ast.NodeType.Break:
                    return GenerateBreak((Ast.BreakNode)node);

                case Ast.NodeType.Continue:
                    return GenerateContinue((Ast.ContinueNode)node);

                case Ast.NodeType.With:
                    return GenerateWith((Ast.WithNode)node);

                case Ast.NodeType.Try:
                    return GenerateTry((Ast.TryNode)node);

                case Ast.NodeType.Throw:
                    return GenerateThrow((Ast.ThrowNode)node);

                case Ast.NodeType.IndexAccess:
                    return GenerateIndexAccess((Ast.IndexAccessNode)node);

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

        private Et GenerateIndexAccess(Ast.IndexAccessNode node)
        {
            return Et.Dynamic(
                new JsGetIndexBinder(new CallInfo(0)),
                typeof(object),
                Generate(node.Target),
                Generate(node.Index)
            );
        }

        private Et GenerateThrow(Ast.ThrowNode node)
        {
            return Et.Throw(
                Generate(node.Target)
            );
        }

        private Et GenerateTry(Ast.TryNode node)
        {
            // try ... finally
            if (node.Catch == null)
            {
                return Et.TryFinally(
                    Generate(node.Body),
                    Generate(node.Finally)
                );
            }
            else
            {
                var catchParam = Et.Parameter(typeof(object), "#catch");

                var catchBody = Et.Block(
                    BuildAssign(
                        node.Catch.Target, 
                        catchParam
                    ),
                    Generate(node.Catch.Body)
                );

                var catchBlock = Et.Catch(
                    catchParam, 
                    catchBody
                );

                var tryBody = EtUtils.Box(Generate(node.Body));

                // try ... catch 
                if (node.Finally == null)
                {
                    return Et.TryCatch(
                        tryBody,
                        catchBlock
                    );
                }
                // try ... catch ... finally
                else
                {
                    return Et.TryCatchFinally(
                        tryBody,
                        Generate(node.Finally),
                        catchBlock
                    );
                }
            }
        }

        private Et GenerateWith(Ast.WithNode node)
        {
            return Et.Block(
                Et.Assign(FrameExpr, 
                    AstUtils.SimpleNewHelper(
                        typeof(WithFrame).GetConstructor(
                            new[] { 
                                typeof(Obj), 
                                typeof(IFrame)
                            }
                        ),
                        Generate(node.Target),
                        FrameExpr
                    )
                ),
                Generate(node.Body),
                FrameUtils.ExitFrame(
                    FrameExpr, 
                    FrameExpr
                )
            );
        }

        // continue
        private Et GenerateContinue(Ast.ContinueNode node)
        {
            return Et.Continue(ContinueLabel);
        }

        // 12.8
        // break, break <label>
        private Et GenerateBreak(Ast.BreakNode node)
        {
            if (node.Label == null)
            {
                return Et.Break(BreakLabel);
            }

            throw new NotImplementedException();
        }

        // 12.6.3
        // for(init, test, incr) { <expr>, <expr> ... <expr> }
        private Et GenerateForStep(Ast.ForStepNode node)
        {
            EnterLoop();
            
            var body = Generate(node.Body);
            var init = Generate(node.Init);
            var incr = Generate(node.Incr);
            var test =
                Et.Dynamic(
                    new JsConvertBinder(typeof(bool)),
                    typeof(bool),
                    Generate(node.Test)
                );

            var loop = AstUtils.Loop(
                test,
                incr,
                body,
                null,
                BreakLabel,
                ContinueLabel
            );

            ExitLoop();

            return Et.Block(
                init,
                loop
            );
        }

        // 12.6.1
        // 12.6.2
        // while(test) { <expr>, <expr> ... <expr> }
        // do { <expr>, <expr> ... <expr> } while(test);
        private Et GenerateWhile(Ast.WhileNode node)
        {
            Et loop;
            EnterLoop();

            var test = Et.Dynamic(
                new JsConvertBinder(typeof(bool)),
                typeof(bool),
                Generate(node.Test)
            );

            // while
            if (node.Loop == Ast.WhileType.While)
            {
                var body = Generate(node.Body);

                loop = AstUtils.While(
                    test,
                    body,
                    null,
                    BreakLabel,
                    ContinueLabel
                );
            }
            // do ... while
            else if (node.Loop == Ast.WhileType.Do)
            {
                var bodyExprs = new List<Et>();

                bodyExprs.Add(Generate(node.Body));

                // test last, instead of first
                bodyExprs.Add(
                    Et.IfThenElse(
                        test,
                        Et.Continue(ContinueLabel),
                        Et.Break(BreakLabel)
                    )
                );

                loop = Et.Loop(
                    Et.Block(
                        bodyExprs
                    ),
                    BreakLabel,
                    ContinueLabel
                );
            }
            else
            {
                throw new NotImplementedException();
            }
            
            ExitLoop();

            return loop;
        }

        // -14 >>> 2
        // 11.7.3
        private Et GenerateUnsignedRightShift(Ast.UnsignedRightShiftNode node)
        {
            //TODO: to much boxing/conversion going on
            return EtUtils.Box(
                Et.Convert(
                    Et.Call(
                        typeof(BuiltIns).GetMethod("UnsignedRightShift"),

                        Et.Convert(
                            Et.Dynamic(
                                new JsConvertBinder(typeof(double)),
                                typeof(double),
                                Generate(node.Left)
                            ),
                            typeof(int)
                        ),

                        Et.Convert(
                            Et.Dynamic(
                                new JsConvertBinder(typeof(double)),
                                typeof(double),
                                Generate(node.Right)
                            ),
                            typeof(int)
                        )

                    ),
                    typeof(double)
                )
            );
        }

        // 11.9.4
        // 11.9.5
        // foo === bar, foo !== bar
        private Et GenerateStrictCompare(Ast.StrictCompareNode node)
        {
            // for both
            Et expr = Et.Call(
                typeof(BuiltIns).GetMethod("StrictEquality"),
                Generate(node.Left),
                Generate(node.Right)
            );

            // specific to 11.9.5
            if(node.Op == ExpressionType.NotEqual)
                expr = Et.Not(Et.Convert(expr, typeof(bool)));

            return expr;
        }

        // 11.12
        // 12.4
        // if (test) { TrueBranch } else { ElseBranch }
        private Et GenerateIf(Ast.IfNode node)
        {
            return Et.Condition(
                Et.Dynamic(
                    new JsConvertBinder(typeof(bool)),
                    typeof(bool),
                    Generate(node.Test)
                ),
                Generate(node.TrueBranch),
                Generate(node.ElseBranch)
            );
        }

        // 11.4.2
        // void foo
        private Et GenerateVoid(Ast.VoidNode node)
        {
            // 11.4.2
            return Et.Block(
                Generate(node.Target),
                Undefined.Expr
            );
        }

        // 8.3
        // true, false
        private Et GenerateBoolean(Ast.BooleanNode node)
        {
            return Et.Constant(
                node.Value, 
                typeof(object)
            );
        }

        // 11.4.3
        // typeof foo
        private Et GenerateTypeOf(Ast.TypeOfNode node)
        {
            return Et.Call(
                typeof(BuiltIns).GetMethod("TypeOf"),
                Generate(node.Target)
            );
        }

        // 11.3
        // foo++, foo--
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
                                         ? 1.0    // 11.3.1
                                         : -1.0,  // 11.3.2
                                typeof(double)
                            )
                        )
                    )
                ),

                tmp // return the old value
            );

            throw new NotImplementedException();
        }
        
        // 11.11
        // foo || bar, foo && bar
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

        // 11.4.6
        // 11.4.7
        // 11.4.8
        // 11.4.9
        // !foo, -foo, etc.
        private Et GenerateUnaryOp(Ast.UnaryOpNode node)
        {
            return Et.Dynamic(
                new JsUnaryOpBinder(node.Op),
                typeof(object),
                Generate(node.Target)
            );
        }

        // 8.2
        // null
        private Et GenerateNull(Ast.NullNode node)
        {
            return Et.Default(typeof(object));
        }

        // 11.2.1
        // foo.bar
        private Et GenerateMemberAccess(Ast.MemberAccessNode node)
        {
            return Et.Dynamic(
                new JsGetMemberBinder(node.Name),
                typeof(object),
                Generate(node.Target)
            );
        }

        // 11.2.2
        // 13.2.2
        // foo = {}
        // foo = new X
        // foo = new X()
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

            // this handles properties defined
            // in the shorthand json-style object
            // expression: { foo: 1, bar: 2 }
            if (node.Properties != null)
            {
                foreach (var propNode in node.Properties)
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
            }

            exprs.Add(
                EtUtils.Box(tmp)
            );

            return Et.Block(
                new[] { tmp },
                exprs
            );
        }

        // 12.8
        private Et GenerateReturn(Ast.ReturnNode node)
        {
            // return <expr>
            return Et.Return(ReturnLabel, Generate(node.Value), typeof(object));
        }

        // 8.4
        private Et GenerateString(Ast.StringNode node)
        {
            // 'foo'
            return Et.Constant(node.Value, typeof(object));
        }

        // 8.5
        private Et GenerateNumber(Ast.NumberNode node)
        {
            // 1.0
            return Et.Constant(node.Value, typeof(object));
        }

        // 7.6
        private Et GenerateIdentifier(Ast.IdentifierNode node, Runtime.Js.GetType type = Runtime.Js.GetType.Value)
        {
            // foo
            return FrameUtils.Pull(FrameExpr, node.Name, type);
        }

        // ???
        private Et GenerateBlock(Ast.BlockNode node)
        {
            // { <expr>, <expr>, ... <expr> }
            if (node.Nodes.Count == 0)
                return Et.Default(typeof(object));

            return Et.Block(
                node.Nodes.Select(x => Generate(x))
            );
        }

        // 11.5
        // 11.6
        // 11.7
        // 11.8
        // 11.9 (not all, strict compare is implemented in GenerateStrictCompare)
        // 11.10
        //TODO: implement 'instanceof' and  'in' operators
        private Et GenerateBinaryOp(Ast.BinaryOpNode node)
        {
            // left @ right
            return Et.Dynamic(
                new JsBinaryOpBinder(node.Op),
                typeof(object),
                Generate(node.Left),
                Generate(node.Right)
            );
        }

        // 13.2.1
        private Et GenerateCall(Ast.CallNode node)
        {
            var args = node.Args.ToEtArray(x => Generate(x));

            // foo();
            if (node.Target is Ast.IdentifierNode)
            {
                var target = GenerateIdentifier(
                    (Ast.IdentifierNode)node.Target, 
                    Runtime.Js.GetType.Call
                );

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

            // foo.bar();
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

        // 13
        private Et GenerateLambda(Ast.LambdaNode node)
        {
            EnterFrame();

            var bodyNode = (Ast.BlockNode)node.Body;
            var argsList = node.Args.Select(x => x.Name).ToList();
            var bodyExprs = bodyNode.Nodes.Select(x => Generate(x)).ToList();

            bodyExprs.Add(
                Et.Label(
                    ReturnLabel, 
                    Undefined.Expr // 12.9
                )
            );

            var lambdaEt = Et.Lambda<Func<IFrame, object>>(
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
                                typeof(IFrame), 
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
                        "#FunctionPrototype",
                        Runtime.Js.GetType.Value
                    )
                ),
                // 3
                tmpObj
            );
        }

        // 11.13.1
        private Et GenerateAssign(Ast.AssignNode node)
        {
            return BuildAssign(node.Target, Generate(node.Value));
        }

        // 11.13.1
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
            else if (target is Ast.IndexAccessNode)
            {
                var ixNode = (Ast.IndexAccessNode)target;

                return Et.Dynamic(
                    new JsSetIndexBinder(new CallInfo(1)),
                    typeof(object),
                    Generate(ixNode.Target),
                    Generate(ixNode.Index),
                    value
                );
            }

            throw new NotImplementedException();
        }
    }
}

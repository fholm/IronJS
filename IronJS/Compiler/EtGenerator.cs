using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using IronJS.Extensions;
using IronJS.Runtime;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Microsoft.Scripting.Utils;

namespace IronJS.Compiler
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = System.Linq.Expressions.Expression;
    using LambdaTuple = Tuple<Expression<LambdaType>, List<string>>;

    public class EtGenerator
    {
        internal Context Context { get; private set; }
        internal FunctionScope FunctionScope { get; private set; }
        internal List<LambdaTuple> LambdaTuples { get; private set; }
        internal ParameterExpression FuncTableExpr { get; private set; }
        internal ParameterExpression GlobalScopeExpr { get; private set; }

        internal int LambdaId { get { return LambdaTuples.Count - 1; } }

        public Action<Scope> Build(List<Ast.Node> astNodes, Context context)
        {
            // Context we're compiling this code for
            Context = context;

            // Holds the uncompiled tuples of Expression<LambdaType> and List<string> of parameter
            LambdaTuples = new List<LambdaTuple>();

            // Function table
            FuncTableExpr = 
                Et.Parameter(
                    typeof(FunctionTable), 
                    "#functbl"
                );

            // Global function scope
            FunctionScope = new FunctionScope();

            // Store the frame expr for global frame
            GlobalScopeExpr = FunctionScope.ScopeExpr;

            // Stores all global expressions
            var globalExprs = new List<Et>();

            // Walk each global node
            foreach (var node in astNodes)
                globalExprs.Add(node.Walk(this));

            return Et.Lambda<Action<Scope>>(
                Et.Block(
                    new[] { FuncTableExpr },

                    // Create instance of our functable
                    Et.Assign(
                        FuncTableExpr,
                        FunctionTable.EtNew()
                    ),

                    // Push functions into functable
                    LambdaTuples.Count > 0 
                      ? (Et) Et.Block(
                            LambdaTuples.Select(x => 
                                FunctionTable.EtPush(
                                    FuncTableExpr,
                                    Lambda.EtNew(
                                        x.V1,
                                        Et.Constant(x.V2.ToArray())
                                    )
                                )
                            )
                        )
                      : (Et) AstUtils.Empty(), // hack

                    // Execute global scope
                    Et.Block(globalExprs)
                ),
                GlobalScopeExpr
            ).Compile();
        }

        internal void EnterFunctionScope()
        {
            FunctionScope = FunctionScope.Enter();
        }

        internal void ExitFunctionScope()
        {
            FunctionScope = FunctionScope.Parent;
        }

        private Et Generate(Ast.Node node)
        {
            if (node is Ast.ILabelableNode)
                (node as Ast.ILabelableNode).Enter(FunctionScope);

            var et = GenerateEt(node);

            if (node is Ast.ILabelableNode)
                (node as Ast.ILabelableNode).Exit(FunctionScope);

            return et;
        }

        private Et GenerateEt(Ast.Node node)
        {
            if (node == null)
                return AstUtils.Empty();

            switch (node.Type)
            {
                case Ast.NodeType.Call:
                    return GenerateCall((Ast.CallNode)node);

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

                case Ast.NodeType.StrictCompare:
                    return GenerateStrictCompare((Ast.StrictCompareNode)node);

                case Ast.NodeType.UnsignedRightShift:
                    return GenerateUnsignedRightShift((Ast.UnsignedRightShiftNode)node);

                case Ast.NodeType.While:
                    return GenerateWhile((Ast.WhileNode)node);

                case Ast.NodeType.ForStep:
                    return GenerateForStep((Ast.ForStepNode)node);

                case Ast.NodeType.ForIn:
                    return GenerateForIn((Ast.ForInNode)node);

                case Ast.NodeType.Break:
                    return GenerateBreak((Ast.BreakNode)node);

                case Ast.NodeType.Continue:
                    return GenerateContinue((Ast.ContinueNode)node);

                /*
                case Ast.NodeType.With:
                    return GenerateWith((Ast.WithNode)node);
                */

                case Ast.NodeType.Try:
                    return GenerateTry((Ast.TryNode)node);

                case Ast.NodeType.Throw:
                    return GenerateThrow((Ast.ThrowNode)node);

                case Ast.NodeType.IndexAccess:
                    return GenerateIndexAccess((Ast.IndexAccessNode)node);

                case Ast.NodeType.Delete:
                    return GenerateDelete((Ast.DeleteNode)node);

                default:
                    throw new Compiler.CompilerError("Unsupported AST node '" + node.Type + "'");
            }
        }

        private Et GenerateDelete(Ast.DeleteNode node)
        {
            if (node.Target is Ast.MemberAccessNode)
            {
                var maNode = (Ast.MemberAccessNode)node.Target;

                return EtUtils.Box(Et.Dynamic(
                    Context.CreateDeleteMemberBinder(maNode.Name),
                    typeof(void),
                    Generate(maNode.Target)
                ));
            }

            if (node.Target is Ast.IndexAccessNode)
            {
                var iaNode = (Ast.IndexAccessNode)node.Target;

                return EtUtils.Box(Et.Dynamic(
                    Context.CreateDeleteIndexBinder(
                        new CallInfo(1)
                    ),
                    typeof(void),
                    Generate(iaNode.Target),
                    Generate(iaNode.Index)
                ));
            }

            throw new NotImplementedException();
        }

        private Et GenerateForIn(Ast.ForInNode node)
        {
            /*
            IObj obj = <node.Source>
            IEnumerator<object> keys = null;
            var set = new HashSet<object>();
            object current = null;

            while (true)
            {
                if (obj == null)
                    break;

                keys = obj.GetAllPropertyNames().GetEnumerator();

                while (true)
                {
                    if (!keys.MoveNext())
                        break;

                    current = keys.Current;

                    if (set.Contains(current))
                        continue;

                    if (obj.HasOwnProperty(current))
                    {
                        set.Add(current);
                        <node.Target> = current;
                        <node.Body>
                    }
                }

                keys.Dispose();
                obj = obj.Prototype;
            }
            */

            // tmp variables
            var obj = Et.Variable(typeof(IObj), "#tmp-forin-obj");
            var keys = Et.Variable(typeof(List<object>.Enumerator), "#tmp-forin-keys"); // IEnumerator<object> keys = null;
            var set = Et.Variable(typeof(HashSet<object>), "#tmp-forin-set");
            var current = Et.Variable(typeof(object), "#tmp-forin-current"); // object current = null;

            // labels
            var innerBreak = Et.Label("#tmp-forin-inner-break");
            var innerContinue = Et.Label("#tmp-forin-inner-continue");
            var outerBreak = Et.Label("#tmp-forin-outer-break");

            return Et.Block(
                new[] { obj, keys, set, current },
                // IObj obj = <node.Source>
                Et.Assign(
                    obj,
                    Et.Dynamic(
                        Context.CreateConvertBinder(typeof(IObj)),
                        typeof(IObj),
                        Generate(node.Source)
                    )
                ),
                // var set = new HashSet<object>();
                Et.Assign(
                    set,
                    AstUtils.SimpleNewHelper(
                        typeof(HashSet<object>).GetConstructor(Type.EmptyTypes)
                    )
                ),
                // while(true) {
                Et.Loop(
                    Et.Block(
                        // if(obj == null) 
                        Et.IfThen( 
                            Et.Equal(obj, Et.Default(typeof(IObj))),
                            // break;
                            Et.Break(outerBreak)
                        ),
                        // keys = obj.GetAllPropertyNames().GetEnumerator();
                        Et.Assign(
                            keys, 
                            Et.Call(
                                Et.Call(
                                    obj,
                                    typeof(IObj).GetMethod("GetAllPropertyNames")
                                ),
                                typeof(List<object>).GetMethod("GetEnumerator")
                            )
                        ),
                        // while(true) {
                        Et.Loop(
                            Et.Block(
                                // if (!keys.MoveNext())
                                Et.IfThen(
                                    Et.Not(
                                        Et.Call(
                                            keys,
                                            typeof(List<object>.Enumerator).GetMethod("MoveNext")
                                        )
                                    ),
                                    // break;
                                    Et.Break(innerBreak)
                                ),
                                // current = keys.Current;
                                Et.Assign(
                                    current,
                                    Et.Property(
                                        keys,
                                        "Current"
                                    )
                                ),
                                // if (set.Contains(current))
                                Et.IfThen(
                                    Et.Call(
                                        set,
                                        typeof(HashSet<object>).GetMethod("Contains"),
                                        current
                                    ),
                                    //  continue;
                                    Et.Continue(innerContinue)
                                ),
                                // if (obj.HasOwnProperty(current)) {
                                Et.IfThen(
                                    Et.Call(
                                        obj,
                                        typeof(IObj).GetMethod("HasOwnProperty"),
                                        current
                                    ),
                                    Et.Block(
                                        // set.Add(current);
                                        Et.Call(
                                            set,
                                            typeof(HashSet<object>).GetMethod("Add"),
                                            current
                                        ),
                                        // <node.Target> = current;
                                        GenerateAssign(
                                            node.Target,
                                            current
                                        ),
                                        // <node.Body>
                                        Generate(node.Body)
                                    )
                                )
                            ),
                            innerBreak,
                            innerContinue
                        ),
                        // keys.Dispose();
                        Et.Call(
                            keys,
                            typeof(List<object>.Enumerator).GetMethod("Dispose")
                        ),
                        // obj = obj.Prototype;
                        Et.Assign(
                            obj,
                            Et.Property(
                                obj,
                                "Prototype"
                            )
                        )
                    ),
                    outerBreak
                )
            );
        }

        private Et GenerateIndexAccess(Ast.IndexAccessNode node)
        {
            return Et.Dynamic(
                Context.CreateGetIndexBinder(new CallInfo(1)),
                typeof(object),
                Generate(node.Target),
                Generate(node.Index)
            );
        }

        private Et GenerateThrow(Ast.ThrowNode node)
        {
            return Et.Throw(
                AstUtils.SimpleNewHelper(
                    typeof(RuntimeError).GetConstructor(new[] { typeof(IObj) }),
                    Generate(node.Target)
                )
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
                    GenerateAssign(
                        node.Catch.Target, 
                        Et.Property(
                            Et.Convert(catchParam, typeof(RuntimeError)),
                            "Obj"
                        )
                    ),
                    Et.Block(
                        Generate(node.Catch.Body)
                    )
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

        /*
        // with(...) { ... }
        private Et GenerateWith(Ast.WithNode node)
        {
            return Et.Block(
                Et.Assign(_functionScope.FrameExpr, 
                    AstUtils.SimpleNewHelper(
                        typeof(WithFrame).GetConstructor(
                            new[] { 
                                typeof(IObj), 
                                typeof(IObj)
                            }
                        ),
                        Generate(node.Target),
                        _functionScope.FrameExpr
                    )
                ),
                Generate(node.Body),
                FrameEtUtils.Exit(
                    _functionScope.FrameExpr,
                    _functionScope.FrameExpr
                )
            );
        }
        */

        // continue
        private Et GenerateContinue(Ast.ContinueNode node)
        {
            if (node.Label == null)
                return Et.Continue(FunctionScope.LabelScope.Continue());

            return Et.Continue(FunctionScope.LabelScope.Continue(node.Label));
        }

        // 12.8
        // break, break <label>
        private Et GenerateBreak(Ast.BreakNode node)
        {
            if (node.Label == null)
                return Et.Break(FunctionScope.LabelScope.Break());

           return Et.Break(FunctionScope.LabelScope.Break(node.Label));
        }

        // 12.6.3
        // for(init, test, incr) { <expr>, <expr> ... <expr> }
        private Et GenerateForStep(Ast.ForStepNode node)
        {
            var body = Generate(node.Body);
            var init = Generate(node.Setup);
            var incr = Generate(node.Incr);
            var test =
                Et.Dynamic(
                    Context.CreateConvertBinder(typeof(bool)),
                    typeof(bool),
                    Generate(node.Test)
                );

            var loop = AstUtils.Loop(
                test,
                incr,
                body,
                null,
                FunctionScope.LabelScope.Break(),
                FunctionScope.LabelScope.Continue()
            );

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

            var test = Et.Dynamic(
                Context.CreateConvertBinder(typeof(bool)),
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
                    FunctionScope.LabelScope.Break(),
                    FunctionScope.LabelScope.Continue()
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
                        Et.Continue(FunctionScope.LabelScope.Continue()),
                        Et.Break(FunctionScope.LabelScope.Break())
                    )
                );

                loop = Et.Loop(
                    Et.Block(
                        bodyExprs
                    ),
                    FunctionScope.LabelScope.Break(),
                    FunctionScope.LabelScope.Continue()
                );
            }
            else
            {
                throw new NotImplementedException();
            }

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
                                Context.CreateConvertBinder(typeof(double)),
                                typeof(double),
                                Generate(node.Left)
                            ),
                            typeof(int)
                        ),

                        Et.Convert(
                            Et.Dynamic(
                                Context.CreateConvertBinder(typeof(double)),
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
                        Context.CreateConvertBinder(typeof(double)),
                        typeof(double),
                        target
                    )
                ),

                // calc new value
                GenerateAssign(
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
                        Context.CreateConvertBinder(typeof(bool)),
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

        // 11.2.1
        // foo.bar
        private Et GenerateMemberAccess(Ast.MemberAccessNode node)
        {
            return Et.Dynamic(
                Context.CreateGetMemberBinder(node.Name),
                typeof(object),
                Et.Dynamic(
                    Context.CreateConvertBinder(typeof(IObj)),
                    typeof(object),
                    Generate(node.Target)
                )
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
            var tmp = Et.Variable(typeof(IObj), "#tmp");
            var exprs = new List<Et>();

            exprs.Add(
                Et.Assign(
                    tmp,
                    EtUtils.Cast<IObj>(
                        Et.Dynamic(
                            Context.CreateInstanceBinder(
                                new CallInfo(args.Length)
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
                            typeof(IObj).GetMethod("Put"),
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
            return Et.Return(FunctionScope.ReturnLabel, Generate(node.Value), typeof(object));
        }

        // 13.2.1
        private Et GenerateCall(Ast.CallNode node)
        {
            var args = node.Args.ToEtArray(x => Generate(x));

            // foo.bar();
            if(node.Target is Ast.MemberAccessNode)
            {
                var target = (Ast.MemberAccessNode)node.Target;
                var tmp = Et.Variable(typeof(object), "#tmp");
                var targetExpr = GenerateConvertToObject(
                        Generate(target.Target)
                    );

                return Et.Block(
                    new[] { tmp },
                    Et.Assign(
                        tmp,
                        targetExpr
                    ),
                    Et.Dynamic(
                        Context.CreateInvokeMemberBinder(
                            target.Name,
                            new CallInfo(args.Length + 1)
                        ),
                        typeof(object),
                        ArrayUtils.Insert(
                            tmp,
                            tmp,
                            args
                        )
                    )
                );
            }

            throw new NotImplementedException();
        }

        private Et GenerateConvertToObject(Et target)
        {
            return Et.Dynamic(
                Context.CreateConvertBinder(typeof(IObj)),
                typeof(object),
                target
            );
        }

        // 11.13.1
        internal Et GenerateAssign(Ast.Node target, Et value)
        {
            if (target is Ast.IdentifierNode)
            {
                var idNode = (Ast.IdentifierNode)target;

                if (idNode.IsLocal)
                {
                    return Scope.EtLocal(
                        FunctionScope.ScopeExpr,
                        idNode.Name,
                        value
                    );
                }
                else
                {
                    return Scope.EtGlobal(
                        FunctionScope.ScopeExpr,
                        idNode.Name,
                        value
                    );
                }
            }
            else if (target is Ast.MemberAccessNode)
            {
                var maNode = (Ast.MemberAccessNode)target;

                return Et.Dynamic(
                    Context.CreateSetMemberBinder(maNode.Name),
                    typeof(object),
                    Generate(maNode.Target),
                    value
                );
            }
            else if (target is Ast.IndexAccessNode)
            {
                var ixNode = (Ast.IndexAccessNode)target;

                return Et.Dynamic(
                    Context.CreateSetIndexBinder(new CallInfo(1)),
                    typeof(object),
                    Generate(ixNode.Target),
                    Generate(ixNode.Index),
                    value
                );
            }

            throw new NotImplementedException();
        }

        internal Et WalkIfNotNull(Ast.Node node)
        {
            if (node != null)
                return node.Walk(this);

            return Et.Default(typeof(object));
        }
    }
}

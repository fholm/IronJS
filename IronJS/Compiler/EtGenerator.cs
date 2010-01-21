using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using IronJS.Runtime;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler
{
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
            return GenerateEt(node);
        }

        private Et GenerateEt(Ast.Node node)
        {
            if (node == null)
                return AstUtils.Empty();

            switch (node.Type)
            {

                case Ast.NodeType.ForIn:
                    return GenerateForIn((Ast.ForInNode)node);

                /*
                case Ast.NodeType.With:
                    return GenerateWith((Ast.WithNode)node);
                */

                default:
                    throw new Compiler.CompilerError("Unsupported AST node '" + node.Type + "'");
            }
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

        internal Et GenerateConvertToObject(Et target)
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
                    maNode.Target.Walk(this),
                    value
                );
            }
            else if (target is Ast.IndexAccessNode)
            {
                var ixNode = (Ast.IndexAccessNode)target;

                return Et.Dynamic(
                    Context.CreateSetIndexBinder(new CallInfo(1)),
                    typeof(object),
                    ixNode.Target.Walk(this),
                    ixNode.Index.Walk(this),
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

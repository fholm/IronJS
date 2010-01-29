using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using IronJS.Runtime;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using System.Reflection;
using System.Reflection.Emit;
using LambdaTuple = System.Tuple<System.Linq.Expressions.Expression<IronJS.Runtime.LambdaType>, System.Collections.Generic.List<string>>;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler
{

    public class EtGenerator
    {
        int _withCount;

        internal Context Context { get; private set; }
        internal FunctionScope FunctionScope { get; private set; }
        internal List<LambdaTuple> LambdaTuples { get; private set; }
        internal ParameterExpression FuncTableExpr { get; private set; }
        internal ParameterExpression GlobalScopeExpr { get; private set; }

        internal bool IsInsideWith { get { return _withCount > 0; } }
        internal int LambdaId { get { return LambdaTuples.Count - 1; } }
        internal bool IsGlobal { get { return FunctionScope == null; } }

        public MethodInfo Build(List<Ast.Node> astNodes, Context context)
        {
            var domain = AppDomain.CurrentDomain;

            // Create dynamic assembly
            var asmName = new AssemblyName("IronJSAssembly");
            var dynAsm = domain.DefineDynamicAssembly(asmName, AssemblyBuilderAccess.RunAndSave);

            // Create a dynamic module and type
            var dynMod = dynAsm.DefineDynamicModule("IronJSModule", "IronJSModule.dll");
            var typeBuilder = dynMod.DefineType("IronJSType");
            
            // Create our method builder for this type builder
            var methodBuilder = typeBuilder.DefineMethod(
                "Global", MethodAttributes.Public | MethodAttributes.Static
            );

            // Context we're compiling this code for
            Context = context;

            // Store the frame expr for global frame
            GlobalScopeExpr = Et.Parameter(typeof(JsObj), "#globals");

            // Stores all global expressions
            var globalExprs = new List<Et>();

            // Walk each global node
            foreach (var node in astNodes)
                globalExprs.Add(node.Generate(this));

            Et.Lambda<Action<JsObj>>(
                Et.Block(
                    globalExprs
                ),
                new[] { GlobalScopeExpr }
            ).CompileToMethod(methodBuilder);

            // Finalize type
            typeBuilder.CreateType();

            // Get created type and method
            var type = dynAsm.GetType("IronJSType");
            return type.GetMethod("Global");
        }

        internal void EnterWith()
        {
            ++_withCount;
        }

        internal void ExitWith()
        {
            --_withCount;
        }

        internal void EnterFunctionScope()
        {
            FunctionScope = (FunctionScope == null)
                            ? new FunctionScope()
                            : FunctionScope.Enter();
        }

        internal void ExitFunctionScope()
        {
            FunctionScope = FunctionScope.Parent;
        }

        internal Et WalkIfNotNull(Ast.Node node)
        {
            if (node != null)
                return node.Generate(this);

            return AstUtils.Empty();
        }

        internal Et BlockIfNotEmpty(IEnumerable<Et> nodes)
        {
            if (nodes.Count() > 0)
                return Et.Block(nodes);

            return AstUtils.Empty();
        }

        internal Et GenerateConvertToObject(Et target)
        {
            return Et.Dynamic(
                Context.CreateConvertBinder(typeof(IObj)),
                typeof(object),
                target
            );
        }

        internal Et Generate<T>(T value)
        {
            return Et.Convert(
                Et.Constant(
                    value,
                    typeof(T)
                ),
                typeof(object)
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
                    return Et.Call(
                        FunctionScope.ScopeExpr,
                        Scope.MiLocal,
                        Et.Constant(idNode.Name, typeof(object)),
                        EtUtils.Cast<object>(value)
                    );
                }
                else
                {
                    return Et.Call(
                        GlobalScopeExpr,
                        typeof(JsObj).GetMethod("Set"),
                        Generate<string>(idNode.Name),
                        EtUtils.Cast<object>(value)
                    );
                }
            }
            else if (target is Ast.MemberAccessNode)
            {
                var maNode = (Ast.MemberAccessNode)target;

                return Et.Dynamic(
                    Context.CreateSetMemberBinder(maNode.Name),
                    typeof(object),
                    maNode.Target.Generate(this),
                    EtUtils.Cast<object>(value)
                );
            }
            else if (target is Ast.IndexAccessNode)
            {
                var ixNode = (Ast.IndexAccessNode)target;

                return Et.Dynamic(
                    Context.CreateSetIndexBinder(new CallInfo(1)),
                    typeof(object),
                    ixNode.Target.Generate(this),
                    ixNode.Index.Generate(this),
                    EtUtils.Cast<object>(value)
                );
            }

            throw new EtCompilerError(
                EtCompilerError.CANT_ASSIGN_TO_NODE_TYPE,
                target.Type
            );
        }
    }
}

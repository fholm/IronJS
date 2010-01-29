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
using EtParam = System.Linq.Expressions.ParameterExpression;

namespace IronJS.Compiler
{

    public class EtGenerator
    {
        int _withCount;

        internal FunctionScope FunctionScope { get; private set; }
        internal List<LambdaTuple> LambdaTuples { get; private set; }
        internal ParameterExpression FuncTableExpr { get; private set; }
        internal bool IsInsideWith { get { return _withCount > 0; } }
        internal int LambdaId { get { return LambdaTuples.Count - 1; } }
        internal bool IsGlobal { get { return FunctionScope == null; } }


        // Build2
        internal Context Context { get; private set; }
        internal ParameterExpression GlobalScopeExpr { get; private set; }
        internal List<Et> GlobalExprs { get; private set; }
        internal LambdaScope LambdaScope { get; set; }
        internal bool IsGlobal2 { get { return LambdaScope == null; } }

        public Action<JsObj> Build2(List<Ast.INode> astNodes, Context context)
        {
            Context = context;
            LambdaScope = null;
            GlobalExprs = new List<Et>();
            GlobalScopeExpr = Et.Parameter(typeof(JsObj), "#globals");

            foreach (var node in astNodes)
                GlobalExprs.Add(node.Generate2(this));

            return Et.Lambda<Action<JsObj>>(
                Et.Block(GlobalExprs),
                new[] { GlobalScopeExpr }
            ).Compile();
        }

        public void Enter()
        {
            if (LambdaScope == null)
                LambdaScope = new LambdaScope(null);
            else
                LambdaScope = LambdaScope.Enter();
        }

        public MethodInfo Build(List<Ast.INode> astNodes, Context context)
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

        internal Et WalkIfNotNull(Ast.INode node)
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
        internal Et GenerateAssign(Ast.INode target, Et value)
        {
            if (target is Ast.IdentifierNode)
            {
                var idNode = (Ast.IdentifierNode)target;

                if (idNode.IsDefinition)
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
                target.NodeType
            );
        }

        internal bool TypesMatch(Ast.IdentifierNode target, Et value)
        {
            if (target.ExprType == Ast.JsType.Integer)
                return value.Type == typeof(int);

            if (target.ExprType == Ast.JsType.String)
                return value.Type == typeof(string);

            return false;
        }

        internal EtParam CreateVariable2(string name, Type type)
        {
            return LambdaScope[name] = Et.Parameter(type, name);
        }

        internal Et GenerateAssign2(Ast.INode target, Et value)
        {
            var idNode = target as Ast.IdentifierNode;
            if (idNode != null)
            {
                if (IsGlobal2)
                {
                    return Et.Call(
                        GlobalScopeExpr,
                        typeof(JsObj).GetMethod("Set"),
                        Et.Constant(idNode.Name),
                        value
                    );
                }
                else
                {
                    var typesMatch = TypesMatch(idNode, value);
                    EtParam variable;

                    if (idNode.IsDefinition)
                    {
                        if (typesMatch)
                            variable = CreateVariable2(idNode.Name, value.Type);
                        else
                            variable = CreateVariable2(idNode.Name, typeof(object));
                    }
                    else
                        variable = LambdaScope[idNode.Name];

                    if (!typesMatch)
                    {
                        if (variable.Type != typeof(object))
                            throw new ArgumentException("Expression types did not mach, but variable.Type is not typeof(object)");

                        return Et.Assign(
                            variable,
                            EtUtils.Box2(value)
                        );
                    }

                    return Et.Assign(
                        variable,
                        value
                    );
                }
            }

            throw new NotImplementedException();
        }
    }
}

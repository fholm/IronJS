using System;
using System.Collections.Generic;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Binders;
using IronJS.Runtime2.Js;
using IronJS.Tools;
using Microsoft.Scripting.Ast;
using Microsoft.Scripting.Utils;
using System.Text;
using IronJS.Runtime2.Js.Proxies;
using IronJS.Compiler.Tools;
using System.Dynamic;

#if CLR2
using Microsoft.Scripting.Ast;
#else
using System.Linq.Expressions;
#endif

namespace IronJS.Compiler.Ast
{
    using AstUtils = Microsoft.Scripting.Ast.Utils;
    using Et = Expression;
    using EtParam = ParameterExpression;

    public class CallNode : Node
    {
        public INode Target { get; protected set; }
        public List<INode> Args { get; protected set; }

        public CallNode(INode target, List<INode> args, ITree node)
            : base(NodeType.Call, node)
        {
            Target = target;
            Args = args;
        }

        public override INode Analyze(FuncNode astopt)
        {
            Target = Target.Analyze(astopt);

            for (int index = 0; index < Args.Count; ++index)
                Args[index] = Args[index].Analyze(astopt);

            IfIdentiferUsedAs(Target, IjsTypes.Object);

            return this;
        }

        public override Et Compile(FuncNode func)
        {
            if (Args.Count == 0)
            {
				return IjsAstTools.Call0(func, Target);
            }
            else
            {
                Et[] args = IEnumerableTools.Map(Args, delegate(INode node) {
                    return node.Compile(func);
                });

                Type callType = typeof(IjsCall1<>).MakeGenericType(
                        IEnumerableTools.Map(args, delegate(Expression expr) {
                            return expr.Type;
                        })
                    );

                Type proxyType = typeof(IjsFunc);
                Type funcType = callType.GetField("Func").FieldType;
                Type guardType = callType.GetField("Guard").FieldType;
                
                Et callExpr = func.GetCallProxy(callType);
                Et proxyField = Et.Field(callExpr, "Proxy");
                Et funcField = Et.Field(callExpr, "Func");
                Et guardField = Et.Field(callExpr, "Guard");

                EtParam tmpN_object = Et.Variable(typeof(object), "__tmpN_object__");
                EtParam tmpN_ijsproxy = Et.Variable(typeof(IjsFunc), "__tmpN_ijsproxy__");
                EtParam tmpN_guard = Et.Variable(guardType, "__tmpN_guard__");


                /*
                 * //variables
                 * $object   // the object we're invoking
                 * $proxy    // our ironjs specific dispatcher proxy
                 * $func     // our ironjs function (this contains the AST for the function + already compiled and cached versions)
                 * 
                 * // logic
                 * if($object is IjsFunc) {
                 * 
                 *    $func = (IjsFunc)$object;
                 *    
                 *    if($proxy.func == $func) {
                 *          if($proxy.guard(arg1, arg2, ...)) {
                 *              $proxy.cache(arg1, arg2, ...);
                 *          } else {
                 *              $proxy.guard = func.compileGuard(arg1, arg2, ...);
                 *              $proxy.cache = func.compile(arg1, arg2, ...);
                 *              $proxy.cache(arg1, arg2, ...);
                 *          }
                 *    } else {
                 *          $proxy.func = $func;
                 *          $proxy.guard = func.compileGuard(arg1, arg2, ...);
                 *          $proxy.cache = func.compile(arg1, arg2, ...);
                 *          $proxy.cache(arg1, arg2, ...);
                 *    }
                 * 
                 * } else {
                 *      // DynamicExpression
                 * }
                 * 
                 * */

                return Et.Block(
                    new[] { tmpN_object },
                    Et.Assign(
                        tmpN_object, Target.Compile(func)
                    ),
                    Et.Condition(
                        Et.TypeIs(
                            tmpN_object, proxyType
                        ),
                        Et.Block(
                            new[] { tmpN_ijsproxy, tmpN_guard },
                            Et.Assign(
                                tmpN_ijsproxy, Et.Convert(tmpN_object, proxyType)
                            ),
                            Et.IfThen(
                                Et.NotEqual(
                                    tmpN_ijsproxy, proxyField
                                ),
                                Et.Block(
                                    Et.Assign(
                                        proxyField, tmpN_ijsproxy
                                    ),
                                    Et.Assign(
                                        funcField,
                                        Et.Call(
                                            proxyField,
                                            typeof(IjsFunc).GetMethod("CompileN").MakeGenericMethod(
                                                funcType, guardType
                                            ),
                                            AstUtils.NewArrayHelper(typeof(object), args),
                                            tmpN_guard
                                        )
                                    ),
                                    Et.Assign(
                                        guardField, tmpN_guard
                                    )
                                )
                            ),
                            Et.Condition(
                                Et.Invoke(
                                    guardField, args
                                ),
                                Et.Invoke(
                                    funcField,
                                    ArrayUtils.Insert(
                                        Et.Field(
                                            tmpN_ijsproxy, "Closure"
                                        ),
                                        args
                                    )
                                ),
								AstTools.Box(
                                    Et.Call(
                                        typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }),
										AstTools.Constant("FAIL")
                                    )
                                )
                            )
                        ),

                        // inter-op function call
                        Et.Dynamic(
                            new IjsInvokeBinder(new CallInfo(Args.Count)),
                            IjsTypes.Dynamic,
                            ArrayUtils.Insert(Target.Compile(func), args)
                        )
                    )
                );
            }
        }

        public override void Print(StringBuilder writer, int indent)
        {
            string indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType);
            Target.Print(writer, indent + 1);

            string indentStr2 = new String(' ', (indent + 1) * 2);

            if (Args.Count > 0)
            {
                writer.AppendLine(indentStr2 + "(Args");

                foreach (INode node in Args)
                    node.Print(writer, indent + 2);

                writer.AppendLine(indentStr2 + ")");
            }
            else
            {
                writer.AppendLine(indentStr2 + "(Args)");
            }

            writer.AppendLine(indentStr + ")");
        }
    }
}

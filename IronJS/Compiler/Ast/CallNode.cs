using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Compiler.Utils;
using IronJS.Runtime2.Binders;
using IronJS.Runtime2.Js;
using IronJS.Runtime2.Js.Proxies;
using Microsoft.Scripting.Utils;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
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

            for (int i = 0; i < Args.Count; ++i)
                Args[i] = Args[i].Analyze(astopt);

            IfIdentiferUsedAs(Target, IjsTypes.Object);

            return this;
        }

        public override Et EtGen(FuncNode func)
        {
            var target = Target as IdentifierNode;

            if (Args.Count == 0)
            {
                var tmp0_object = Et.Variable(typeof(object), "__tmp0_object__");
                var tmp0_ijsproxy = Et.Variable(typeof(IjsProxy), "__tmp0_ijsproxy__");

                return Et.Block(
                    new[] { tmp0_object },
                    Et.Assign(
                        tmp0_object,
                        target.EtGen(func)
                    ),
                    Et.Condition(
                        Et.TypeIs(tmp0_object, typeof(IjsProxy)),
                        Et.Block(
                            new[] { tmp0_ijsproxy },
                            Et.Assign(
                                tmp0_ijsproxy,
                                Et.Convert(tmp0_object, typeof(IjsProxy))
                            ),
                            Et.Condition(
                                Et.Equal(
                                    Et.Field(tmp0_ijsproxy, "Func0"),
                                    Et.Constant(null, typeof(object))
                                ),
                                Et.Call(
                                    tmp0_ijsproxy,
                                    typeof(IjsProxy).GetMethod("Invoke0")
                                ),
                                Et.Invoke(
                                    Et.Field(tmp0_ijsproxy, "Func0"),
                                    Et.Field(tmp0_ijsproxy, "Closure")
                                )
                            )
                        ),
                        Et.Dynamic(
                            new IjsInvokeBinder(new CallInfo(Args.Count)),
                            IjsTypes.Dynamic,
                            Target.EtGen(func)
                        )
                    )
                );
            }
            else
            {
                var args = Args.Select(x => x.EtGen(func)).ToArray();

                var callType = typeof(IjsCall1<>).MakeGenericType(args.Select(x => x.Type).ToArray());
                var proxyType = typeof(IjsProxy);
                var funcType = callType.GetField("Func").FieldType;
                var guardType = callType.GetField("Guard").FieldType;
                
                var callExpr = func.GetCallProxy(callType);
                var proxyField = Et.Field(callExpr, "Proxy");
                var funcField = Et.Field(callExpr, "Func");
                var guardField = Et.Field(callExpr, "Guard");

                var tmpN_object = Et.Variable(typeof(object), "__tmpN_object__");
                var tmpN_ijsproxy = Et.Variable(typeof(IjsProxy), "__tmpN_ijsproxy__");
                var tmpN_guard = Et.Variable(typeof(Delegate), "__tmpN_guard__");


                /*
                 * 
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
                        tmpN_object, target.EtGen(func)
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
                                        Et.Convert(
                                            Et.Call(
                                                proxyField,
                                                typeof(IjsProxy).GetMethod("CreateN"),
                                                Et.Constant(funcType, typeof(Type)),
                                                AstUtils.NewArrayHelper(typeof(object), args),
                                                tmpN_guard
                                            ),
                                            funcType
                                        )
                                    ),
                                    Et.Assign(
                                        guardField,
                                        Et.Convert(tmpN_guard, guardType)
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
                                IjsEtGenUtils.Box(
                                    Et.Call(
                                        typeof(Console).GetMethod("WriteLine", new[] { typeof(string) }),
                                        IjsEtGenUtils.Constant("FAIL")
                                    )
                                )
                            )
                        ),

                        // inter-op function call
                        Et.Dynamic(
                            new IjsInvokeBinder(new CallInfo(Args.Count)),
                            IjsTypes.Dynamic,
                            ArrayUtils.Insert(Target.EtGen(func), args)
                        )
                    )
                );
            }
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);

            writer.AppendLine(indentStr + "(" + NodeType);
            Target.Print(writer, indent + 1);

            var indentStr2 = new String(' ', (indent + 1) * 2);

            if (Args.Count > 0)
            {
                writer.AppendLine(indentStr2 + "(Args");

                foreach (var node in Args)
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

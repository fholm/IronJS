using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Compiler.Utils;
using IronJS.Runtime2.Binders;
using IronJS.Runtime2.Js;
using Microsoft.Scripting.Utils;
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

            var tmp = Et.Variable(typeof(object), "__tmp__");
            var tmp2 = Et.Variable(typeof(IjsProxy), "__tmp2__");

            return Et.Block(
                new[] { tmp },
                Et.Assign(
                    tmp, target.EtGen(func)
                ),
                Et.Condition(
                    Et.TypeIs(tmp, typeof(IjsProxy)),
                    Et.Block(
                        new[] { tmp2 },
                        Et.Assign(
                            tmp2, Et.Convert(tmp, typeof(IjsProxy))
                        ),
                        Et.Condition(
                            Et.Equal(
                                Et.Field(tmp2, "_func" + Args.Count),
                                Et.Constant(null, typeof(object))
                            ),
                            Et.Call(
                                tmp2,
                                typeof(IjsProxy).GetMethod("Call" + Args.Count),
                                Args.Select(x => IjsEtGenUtils.Box(x.EtGen(func)))
                            ),
                            Et.Condition(
                                Et.Invoke(
                                    Et.Field(tmp2, "_func" + Args.Count + "Guard"),
                                    Args.Select(x => IjsEtGenUtils.Box(x.EtGen(func))).ToArray()
                                ),
                                Et.Invoke(
                                    Et.Field(tmp2, "_func" + Args.Count),
                                    ArrayUtils.Insert(
                                        Et.Field(tmp2, "Closure"),
                                        Args.Select(x => IjsEtGenUtils.Box(x.EtGen(func))).ToArray()
                                    )
                                ),
                                Et.Call(
                                    tmp2,
                                    typeof(IjsProxy).GetMethod("Call" + Args.Count),
                                    Args.Select(x => IjsEtGenUtils.Box(x.EtGen(func)))
                                )
                            )
                        )
                    ),
                    Et.Dynamic(
                        new IjsInvokeBinder(new CallInfo(Args.Count)),
                        IjsTypes.Dynamic,
                        ArrayUtils.Insert(
                            Target.EtGen(func),
                            Args.Select(x => x.EtGen(func)).ToArray()
                        )
                    )
                )
            );
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

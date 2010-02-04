using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime2.Binders;
using IronJS.Runtime2.Js;
using Microsoft.Scripting.Utils;
using Et = System.Linq.Expressions.Expression;
using IronJS.Compiler.Utils;
using System.Reflection;

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

            return Et.Block(
                new[] { tmp },
                Et.Assign(
                    tmp,
                    target.EtGen(func)
                ),
                Et.Condition(
                    Et.TypeIs(
                        tmp,
                        typeof(IjsProxy)
                    ),
                    Et.Block(
                        Et.Call(
                            Et.Convert(tmp, typeof(IjsProxy)),
                            typeof(IjsProxy).GetMethod("Call" + Args.Count),
                            Args.Select(x => IjsEtGenUtils.Box(x.EtGen(func)))
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

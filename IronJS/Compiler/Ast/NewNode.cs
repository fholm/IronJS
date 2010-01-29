using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using Antlr.Runtime.Tree;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Microsoft.Scripting.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class NewNode : Node
    {
        public INode Target { get; protected set; }
        public List<INode> Args { get; protected set; }

        public NewNode(INode target, List<INode> args, ITree node)
            : base(NodeType.New, node)
        {
            Target = target;
            Args = args;
        }

        public NewNode(INode target, ITree node)
            : this(target, new List<INode>(), node)
        {

        }

        public override JsType ExprType
        {
            get
            {
                return JsType.Object;
            }
        }

        public override Et Generate(EtGenerator etgen)
        {
            var target = Target.Generate(etgen);
            var args = Args.Select(x => x.Generate(etgen)).ToArray();
            var tmp = Et.Variable(typeof(IObj), "#tmp");
            var exprs = new List<Et>();

            return Et.Block(
                new[] { tmp },
                Et.Assign(
                    tmp,
                    EtUtils.Cast<IObj>(
                        Et.Dynamic(
                            etgen.Context.CreateInstanceBinder(
                                new CallInfo(args.Length)
                            ),
                            typeof(object),
                            ArrayUtils.Insert(
                                target,
                                args
                            )
                        )
                    )
                ),
                EtUtils.CreateBlockIfNotEmpty(exprs),
                EtUtils.Box(tmp)
            );
        }

        public override void Print(StringBuilder writer, int indent = 0)
        {
            var indentStr = new String(' ', indent * 2);
            var indentStr2 = new String(' ', (indent + 1) * 2);

            writer.AppendLine(indentStr + "(" + NodeType);

            writer.AppendLine(indentStr2 + "(Args");
            foreach (var arg in Args)
                arg.Print(writer, indent + 2);
            writer.AppendLine(indentStr2 + ")");

            Target.Print(writer, indent + 1);

            writer.AppendLine(indentStr + ")");
        }
    }
}

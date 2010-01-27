using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using Microsoft.Scripting.Utils;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Compiler.Ast
{
    public class NewNode : Node
    {
        public Node Target { get; protected set; }
        public List<Node> Args { get; protected set; }
        public List<AutoPropertyNode> Properties { get; protected set; }

        public bool HasProperties { get { return Properties != null; } }

        public NewNode(Node target, List<Node> args)
            : base(NodeType.New)
        {
            Target = target;
            Args = args;
        }

        public NewNode(Node target)
            : this(target, new List<Node>())
        {

        }

        public NewNode(Node targets, List<Node> args, List<AutoPropertyNode> properties)
            : this(targets, args)
        {
            Properties = properties;
        }

        public override Et Walk(EtGenerator etgen)
        {
            var target = Target.Walk(etgen);
            var args = Args.Select(x => x.Walk(etgen)).ToArray();
            var tmp = Et.Variable(typeof(IObj), "#tmp");
            var exprs = new List<Et>();

            // this handles properties defined
            // in the shorthand json-style object
            // expression: { foo: 1, bar: 2 }
            if (HasProperties)
            {
                foreach (var prop in Properties)
                {
                    exprs.Add(
                        Et.Call(
                            tmp,
                            IObjMethods.MiPut,
                            Et.Constant(prop.Name, typeof(object)),
                            EtUtils.Box(prop.Value.Walk(etgen))
                        )
                    );
                }
            }

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
    }
}

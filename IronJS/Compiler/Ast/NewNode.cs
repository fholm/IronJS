using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Scripting.Utils;
using IronJS.Runtime.Js;

using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Restrict = System.Dynamic.BindingRestrictions;
using EtParam = System.Linq.Expressions.ParameterExpression;
using IronJS.Runtime.Utils;

namespace IronJS.Compiler.Ast
{
    class NewNode : Node
    {
        public readonly Node Target;
        public readonly List<Node> Args;
        public readonly List<AutoPropertyNode> Properties;

        public bool HasProperties { get { return Properties != null; } }

        public NewNode(Node target, List<Node> args)
            : base(NodeType.New)
        {
            Target = target;
            Args = args;
        }

        public NewNode(IdentifierNode target)
            : this(target, new List<Node>())
        {

        }

        public NewNode(IdentifierNode targets, List<Node> args, List<AutoPropertyNode> properties)
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
                            typeof(IObj).GetMethod("Put"),
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
                EtUtils.CreateBlockIfNotEmpt(exprs),
                EtUtils.Box(tmp)
            );
        }
    }
}

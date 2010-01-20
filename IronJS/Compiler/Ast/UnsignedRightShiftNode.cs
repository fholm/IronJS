using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Microsoft.Scripting.Utils;
using IronJS.Runtime.Js;
using IronJS.Runtime.Utils;
using IronJS.Runtime;

using Et = System.Linq.Expressions.Expression;
using Meta = System.Dynamic.DynamicMetaObject;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Restrict = System.Dynamic.BindingRestrictions;
using EtParam = System.Linq.Expressions.ParameterExpression;

namespace IronJS.Compiler.Ast
{
    class UnsignedRightShiftNode : Node
    {
        public readonly Node Left;
        public readonly Node Right;

        public UnsignedRightShiftNode(Node left, Node right)
            : base(NodeType.UnsignedRightShift)
        {
            Left = left;
            Right = right;
        }

        public override Et Walk(EtGenerator etgen)
        {
            //TODO: to much boxing/conversion going on
            return EtUtils.Box(
                Et.Convert(
                    Et.Call(
                        typeof(BuiltIns).GetMethod("UnsignedRightShift"),

                        Et.Convert(
                            Et.Dynamic(
                                etgen.Context.CreateConvertBinder(typeof(double)),
                                typeof(double),
                                Left.Walk(etgen)
                            ),
                            typeof(int)
                        ),

                        Et.Convert(
                            Et.Dynamic(
                                etgen.Context.CreateConvertBinder(typeof(double)),
                                typeof(double),
                                Right.Walk(etgen)
                            ),
                            typeof(int)
                        )

                    ),
                    typeof(double)
                )
            );
        }
    }
}

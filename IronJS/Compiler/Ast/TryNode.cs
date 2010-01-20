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
using IronJS.Runtime;
using IronJS.Runtime.Utils;

namespace IronJS.Compiler.Ast
{
    class TryNode : Node
    {
        public readonly Node Body;
        public readonly CatchNode Catch;
        public readonly Node Finally;

        public TryNode(Node body, CatchNode _catch, Node _finally)
            : base(NodeType.Try)
        {
            Body = body;
            Catch = _catch;
            Finally = _finally;
        }

        public override Et Walk(EtGenerator etgen)
        {
            // try ... finally
            if (Catch == null)
            {
                return Et.TryFinally(
                    Body.Walk(etgen),
                    Finally.Walk(etgen)
                );
            }
            else
            {
                var catchParam = Et.Parameter(typeof(object), "#catch");

                var catchBody = Et.Block(
                    etgen.GenerateAssign(
                        Catch.Target,
                        Et.Property(
                            Et.Convert(catchParam, typeof(JsRuntimeError)),
                            "JsObj"
                        )
                    ),
                    Et.Block(
                        Catch.Body.Walk(etgen)
                    )
                );

                var catchBlock = Et.Catch(
                    catchParam,
                    catchBody//,
                    //Et.TypeIs(catchParam, typeof(JsRuntimeError))
                );

                var tryBody = EtUtils.Box(Body.Walk(etgen));

                // try ... catch 
                if (Finally == null)
                {
                    return Et.TryCatch(
                        tryBody,
                        catchBlock
                    );
                }
                // try ... catch ... finally
                else
                {
                    return Et.TryCatchFinally(
                        tryBody,
                        Finally.Walk(etgen),
                        catchBlock
                    );
                }
            }
        }
    }
}

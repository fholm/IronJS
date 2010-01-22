using System;
using AstUtils = Microsoft.Scripting.Ast.Utils;
using Et = System.Linq.Expressions.Expression;
using IronJS.Runtime.Js;
using System.Collections.Generic;

namespace IronJS.Compiler.Ast
{
    // 12.6.4
    class ForInNode : LoopNode
    {
        public readonly Node Target;
        public readonly Node Source;
        public readonly Node Body;

        public ForInNode(Node target, Node source, Node body)
            : base(NodeType.ForIn)
        {
            Target = target;
            Source = source;
            Body = body;
        }

        public override Et LoopWalk(EtGenerator etgen)
        {
            /*
            IObj obj = <node.Source>
            IEnumerator<object> keys = null;
            var set = new HashSet<object>();
            object current = null;

            while (true)
            {
                if (obj == null)
                    break;

                keys = obj.GetAllPropertyNames().GetEnumerator();

                while (true)
                {
                    if (!keys.MoveNext())
                        break;

                    current = keys.Current;

                    if (set.Contains(current))
                        continue;

                    if (obj.HasOwnProperty(current))
                    {
                        set.Add(current);
                        <node.Target> = current;
                        <node.Body>
                    }
                }

                keys.Dispose();
                obj = obj.Prototype;
            }
            */

            // tmp variables
            var obj = Et.Variable(typeof(IObj), "#tmp-forin-obj");
            var keys = Et.Variable(typeof(List<object>.Enumerator), "#tmp-forin-keys"); // IEnumerator<object> keys = null;
            var set = Et.Variable(typeof(HashSet<object>), "#tmp-forin-set");
            var current = Et.Variable(typeof(object), "#tmp-forin-current"); // object current = null;

            // labels
            var innerBreak = Et.Label("#tmp-forin-inner-break");
            var innerContinue = etgen.FunctionScope.LabelScope.Continue();
            var outerBreak = etgen.FunctionScope.LabelScope.Break();

            return Et.Block(
                new[] { obj, keys, set, current },

                // IObj obj = <node.Source>
                Et.Assign(
                    obj,
                    Et.Dynamic(
                        etgen.Context.CreateConvertBinder(typeof(IObj)),
                        typeof(IObj),
                        Source.Walk(etgen)
                    )
                ),
                // var set = new HashSet<object>();
                Et.Assign(
                    set,
                    AstUtils.SimpleNewHelper(
                        typeof(HashSet<object>).GetConstructor(System.Type.EmptyTypes)
                    )
                ),

                // while(true) {
                Et.Loop(
                    Et.Block(
                        // if(obj == null) 
                        Et.IfThen(
                            Et.Equal(obj, Et.Default(typeof(IObj))),
                            // break;
                            Et.Break(outerBreak)
                        ),

                        // keys = obj.GetAllPropertyNames().GetEnumerator();
                        Et.Assign(
                            keys,
                            Et.Call(
                                Et.Call(
                                    obj,
                                    IObjMethods.MiGetAllPropertyNames
                                ),
                                typeof(List<object>).GetMethod("GetEnumerator")
                            )
                        ),

                        // while(true) {
                        Et.Loop(
                            Et.Block(

                                // if (!keys.MoveNext())
                                Et.IfThen(
                                    Et.Not(
                                        Et.Call(
                                            keys,
                                            typeof(List<object>.Enumerator).GetMethod("MoveNext")
                                        )
                                    ),
                                    // break;
                                    Et.Break(innerBreak)
                                ),

                                // current = keys.Current;
                                Et.Assign(
                                    current,
                                    Et.Property(
                                        keys,
                                        "Current"
                                    )
                                ),

                                // if (set.Contains(current))
                                Et.IfThen(
                                    Et.Call(
                                        set,
                                        typeof(HashSet<object>).GetMethod("Contains"),
                                        current
                                    ),
                                    //  continue;
                                    Et.Continue(innerContinue)
                                ),

                                // if (obj.HasOwnProperty(current)) {
                                Et.IfThen(
                                    Et.Call(
                                        obj,
                                        IObjMethods.MiHasOwnProperty,
                                        current
                                    ),
                                    Et.Block(

                                        // set.Add(current);
                                        Et.Call(
                                            set,
                                            typeof(HashSet<object>).GetMethod("Add"),
                                            current
                                        ),

                                        // <node.Target> = current;
                                        etgen.GenerateAssign(
                                            Target,
                                            current
                                        ),

                                        // <node.Body>
                                        Body.Walk(etgen)
                                    )
                                )
                            ),
                            innerBreak,
                            innerContinue
                        ),

                        // keys.Dispose();
                        Et.Call(
                            keys,
                            typeof(List<object>.Enumerator).GetMethod("Dispose")
                        ),

                        // obj = obj.Prototype;
                        Et.Assign(
                            obj,
                            Et.Property(
                                obj,
                                "Prototype"
                            )
                        )
                    ),
                    outerBreak
                )
            );
        }
    }
}

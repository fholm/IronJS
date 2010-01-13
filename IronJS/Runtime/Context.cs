using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Js;

namespace IronJS.Runtime
{
    public class Context
    {
        public IFrame SuperGlobals;

        public Obj Object { get; protected set; }
        public Obj ObjectPrototype { get; protected set; }
        public Obj Function { get; protected set; }
        public Obj FunctionPrototype { get; protected set; }

        protected Context()
        {

        }

        public IFrame Run(Action<IFrame> delegat)
        {
            var globals = new Frame(SuperGlobals, true);

            delegat(globals);

            return globals;
        }

        static public Context Setup()
        {
            var ctx = new Context();

            ctx.SuperGlobals = new Frame();
            ctx.ObjectPrototype = new Obj();
            ctx.FunctionPrototype = new Obj(
                ctx.SuperGlobals,
                new Lambda(
                    new Func<IFrame, object>(FunctionPrototypeLambda),
                    new string[] { }.ToList()
                )
            );

            ctx.Object = new Obj(
                ctx.SuperGlobals, 
                new Lambda(
                    new Func<IFrame, object>(ObjectConstructorLambda),
                    new[] { "value" }.ToList()
                )
            );

            ctx.Function = new Obj(
                ctx.SuperGlobals,
                new Lambda(
                    new Func<IFrame, object>(FunctionConstructorLambda),
                    new string[] { }.ToList()
                )
            );

            // Object
            ctx.Object.Prototype = ctx.FunctionPrototype;
            ctx.Object.SetOwnProperty("prototype", ctx.ObjectPrototype);

            // Function
            ctx.Function.Prototype = ctx.FunctionPrototype;
            ctx.Function.SetOwnProperty("prototype", ctx.FunctionPrototype);

            // Function.prototype
            ctx.FunctionPrototype.Prototype = ctx.ObjectPrototype;
            ctx.FunctionPrototype.SetOwnProperty("constructor", ctx.Function);

            // Push on global frame
            ctx.SuperGlobals.Push("Object", ctx.Object, VarType.Global);
            ctx.SuperGlobals.Push("Function", ctx.Function, VarType.Global);
            ctx.SuperGlobals.Push("undefined", Js.Undefined.Instance, VarType.Global);
            ctx.SuperGlobals.Push("Infinity", double.PositiveInfinity, VarType.Global);
            ctx.SuperGlobals.Push("NaN", double.NaN, VarType.Global);

            return ctx;
        }

        static public object FunctionPrototypeLambda(IFrame frame)
        {
            return Js.Undefined.Instance;
        }

        static public object FunctionConstructorLambda(IFrame frame)
        {
            return null;
        }

        static public object ObjectConstructorLambda(IFrame frame)
        {
            var value = frame.Arg("value");

            if (value != null || value == Js.Undefined.Instance)
            {
                throw new NotImplementedException("ToObject() not implemented");
            }

            return null;
        }
    }
}

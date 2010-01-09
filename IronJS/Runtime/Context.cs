using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using IronJS.Runtime.Js;

namespace IronJS.Runtime
{
    public class Context
    {
        public Frame Globals { get; protected set; }
        public Obj Object { get; protected set; }
        public Obj ObjectPrototype { get; protected set; }
        public Obj Function { get; protected set; }
        public Obj FunctionPrototype { get; protected set; }

        public Context()
        {
        }

        static public Context Setup()
        {
            var ctx = new Context();

            ctx.Globals = new Frame();

            ctx.ObjectPrototype = new Obj();
            ctx.FunctionPrototype = new Obj();

            ctx.Object = new Obj(
                ctx.Globals, 
                new Lambda(
                    new Func<Frame, object>(ObjectConstructor),
                    new[] { "value" }.ToList()
                )
            );

            ctx.Function = new Obj(
                ctx.Globals,
                new Lambda(
                    new Func<Frame, object>(FunctionConstructor),
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
            ctx.Globals.Push("Object", ctx.Object, VarType.Global);
            ctx.Globals.Push("Function", ctx.Function, VarType.Global);
            ctx.Globals.Push("undefined", Js.Undefined.Instance, VarType.Global);
            ctx.Globals.Push("Infinity", double.PositiveInfinity, VarType.Global);
            ctx.Globals.Push("NaN", Js.Nan.Instance, VarType.Global);

            // Conveniance globals
            ctx.Globals.Push("#ObjectPrototype", ctx.ObjectPrototype, VarType.Global);
            ctx.Globals.Push("#FunctionPrototype", ctx.FunctionPrototype, VarType.Global);

            return ctx;
        }

        static public object FunctionConstructor(Frame frame)
        {
            return null;
        }

        static public object ObjectConstructor(Frame frame)
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

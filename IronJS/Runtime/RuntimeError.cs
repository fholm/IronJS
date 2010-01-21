using System;
using IronJS.Runtime.Js;
using System.Reflection;
using Et = System.Linq.Expressions.Expression;

namespace IronJS.Runtime
{
    public abstract class RuntimeError : IronJS.Error
    {
        public RuntimeError(string msg)
            : base(msg)
        {

        }
    }

    public class JsRuntimeError : RuntimeError
    {
        static public readonly ConstructorInfo Ctor = 
            typeof(JsRuntimeError).GetConstructor(new[] { typeof(IObj) });

        public IObj JsObj { get; protected set; }

        public JsRuntimeError(IObj obj)
            : base("An uncaught javascript exception has occured")
        {
            JsObj = obj;
        }

        static public Et EtNew(string message)
        {
            var tmp = Et.Parameter(typeof(IObj), "#tmp");

            return Et.Block(
                new[] { tmp },
                Et.Assign(
                    tmp,
                    Et.New(Obj.Ctor)
                ),
                IObjMethods.EtSetOwnProperty(
                    tmp,
                    "message",
                    Et.Constant(message)
                ),
                Et.Throw(
                    Et.New(
                        Ctor,
                        tmp
                    ),
                    typeof(JsRuntimeError)
                )
            );
        }
    }

    public class InternalRuntimeError : RuntimeError
    {
        static public readonly ConstructorInfo Ctor =
            typeof(InternalRuntimeError).GetConstructor(new[] { typeof(string) });

        public InternalRuntimeError(string msg)
            : base(msg)
        {

        }

        static public Et EtNew(string message)
        {
            return Et.Throw(
                Et.New(
                    Ctor,
                    Et.Constant(message, typeof(string))
                ),
                typeof(InternalRuntimeError)
            );
        }
    }

}

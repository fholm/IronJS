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

    public class ShouldThrowTypeError : RuntimeError
    {
        public ShouldThrowTypeError()
            : base("")
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
                IObjUtils.EtSetOwnProperty(
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

    sealed public class InternalRuntimeError : RuntimeError
    {
        public const string NOT_DEFINED = "Variable '{0}' is not defined";
        public const string NOT_CALLABLE = "Variable '{0}' is not callable";
        public const string PROPERTY_READONLY = "Property is read-only";

        private InternalRuntimeError(string msg)
            : base(msg)
        {

        }

        static public InternalRuntimeError New(string message, params object[] values)
        {
            return new InternalRuntimeError(String.Format(message, values));
        }

        static public Et EtNew(string message, params object[] values)
        {
            return Et.Throw(
                Et.Call(
                    typeof(InternalRuntimeError).GetMethod("New"),
                    Et.Constant(message),
                    Et.Constant(values)
                ),
                typeof(InternalRuntimeError)
            );
        }
    }

}

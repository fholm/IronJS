using System;
using IronJS.Runtime.Js;
using System.Reflection;

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
    }

    public class InternalRuntimeError : RuntimeError
    {
        public InternalRuntimeError(string msg)
            : base(msg)
        {

        }
    }

}

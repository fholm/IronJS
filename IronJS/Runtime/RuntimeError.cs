using System;
using IronJS.Runtime.Js;

namespace IronJS.Runtime
{
    internal class RuntimeError : IronJS.Error
    {
        public IObj Obj { get; protected set; }

        public RuntimeError(IObj obj)
            : base("An uncaught javascript exception has occured")
        {
            Obj = obj;
        }

        public RuntimeError(string msg)
            : base("IronJS.Runtime.Error: " + msg)
        {

        }

        public RuntimeError(string msg, params object[] args)
            : this(String.Format(msg, args))
        {

        }
    }
}

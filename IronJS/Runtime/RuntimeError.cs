using System;

namespace IronJS.Runtime
{
    internal class RuntimeError : IronJS.Error
    {
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

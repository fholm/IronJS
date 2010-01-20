using System;

namespace IronJS.Compiler
{
    internal class CompilerError : IronJS.Error
    {
        internal CompilerError(string msg)
            : base("IronJS.Compiler.Error: " + msg)
        {

        }
    }
}

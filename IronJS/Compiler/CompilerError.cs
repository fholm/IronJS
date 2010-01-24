using System;

namespace IronJS.Compiler
{
    public class CompilerError : Error
    {
        internal CompilerError(string msg)
            : base(msg)
        {

        }

        internal CompilerError(string msg, params object[] parms)
            : this(String.Format(msg, parms))
        {

        }
    }

    public class AstCompilerError : CompilerError
    {
        internal AstCompilerError(string msg, params object[] parms)
            : base(msg, parms)
        {

        }
    }

    public class EtCompilerError : CompilerError
    {
        internal const string CANT_ASSIGN_TO_NODE_TYPE = "Can't assign to node of type '{0}'";

        internal EtCompilerError(string msg, params object[] parms)
            : base(msg, parms)
        {

        }
    }
}

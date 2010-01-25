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
        internal const string NOT_INSIDE_LOOP = "Not inside a loop";
        internal const string NO_CONTINUE_LABEL_NAMED = "No loop labelled '{0}' found (continue only works with loops)";
        internal const string NO_LABEL_NAMED = "No label named '{0}' found";

        internal EtCompilerError(string msg, params object[] parms)
            : base(msg, parms)
        {

        }
    }
}

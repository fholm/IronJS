using System;

using Antlr.Runtime.Tree;

namespace IronJS.Compiler
{
    internal class CompilerError : IronJS.Error
    {
        internal CompilerError(string msg)
            : base("IronJS.Compiler.Error: " + msg)
        {

        }

        internal CompilerError(string msg, ITree node)
            : this(msg + " on line: " + node.Line + " char: " + node.CharPositionInLine)
        {

        }

        internal CompilerError(string msg, ITree node, params object[] args)
            : this(String.Format(msg, args), node)
        {

        }
    }
}

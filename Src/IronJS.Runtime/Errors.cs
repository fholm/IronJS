using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Runtime
{
    public abstract class Error : Exception
    {
        public Error(string message)
            : base(message)
        {

        }
    }

    public class CompilerError : Error
    {
        public Tuple<int, int> Position { get; private set; }
        public string Source { get; private set; }
        public string Path { get; private set; }

        public CompilerError(string message, Tuple<int, int> position, string source, string path)
            : base(message)
        {
            Position = position ?? Tuple.Create(0, 0);
            Source = source ?? "<unknown>";
            Path = path ?? "<unknown>";
        }
    }

    public class RuntimeError : Error
    {
        public RuntimeError(string message)
            : base(message)
        {

        }
    }
}

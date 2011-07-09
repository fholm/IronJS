using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Runtime
{
    internal static class SourceCodePrinter
    {
        private static string[] SplitLines(string input)
        {
            var cleanedInput = (input ?? "").Replace("\r\n", "\n").Replace("\r", "\n");
            return System.Text.RegularExpressions.Regex.Split(cleanedInput, "\n");
        }

        private static string LineNumber(int padding, int input)
        {
            return (input.ToString()).PadLeft(padding, '0');
        }

        private static string MakeArrow(int length)
        {
            var builder = new StringBuilder(length).Insert(0, "-", length);
            return builder + "^";
        }

        internal static string PrettyPrintSourceError(Tuple<int, int> aboveBelow, Tuple<int, int> lineCol, string source)
        {
            var above = aboveBelow.Item1;
            var below = aboveBelow.Item2;
            var line = lineCol.Item1;
            var column = lineCol.Item2;

            //TODO: Implement Source Code Pretty Printer

            throw new NotImplementedException();
        }
    }

    public abstract class Error : Exception
    {
        public Error(string message)
            : base(message ?? "<unknown>")
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

        //TODO: Implement Raise methods
    }

    public class RuntimeError : Error
    {
        public RuntimeError(string message)
            : base(message)
        {

        }

        //TODO: Implement Raise methods
    }

    public class UserError : Error
    {
        public BoxedValue Value { get; private set; }
        public int Line { get; private set; }
        public int Column { get; private set; }

        public UserError(BoxedValue value, int line, int column)
            : base(TypeConverter.ToString(value))
        {
            Value = value;
            Line = line;
            Column = column;
        }
    }
}

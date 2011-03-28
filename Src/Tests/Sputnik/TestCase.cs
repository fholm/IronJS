using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace IronJS.Tests.Sputnik
{
    public class TestCase
    {
        public readonly string FullPath;
        public readonly string RelativePath;
        public readonly string TestName;
        public readonly string[] Sections;
        public readonly string Assertion;
        public readonly string Description;
        public readonly bool Negative;

        public TestCase(string basePath, string path)
        {
            this.FullPath = path;
            this.RelativePath = path.Substring(basePath.Length);

            var text = File.ReadAllText(path);
            this.TestName = Path.GetFileNameWithoutExtension(path);
            this.Sections = Regex.Split(Regex.Match(text, "@section: ([^;]+);").Groups[1].Value, ", ");
            this.Assertion = CleanComment(Regex.Match(text, "@assertion: ([^;]+);").Groups[1].Value);
            this.Description = CleanComment(Regex.Match(text, "@description: ([^;]+);").Groups[1].Value);
            this.Negative = Regex.Match(text, "@negative").Success;
        }

        private string CleanComment(string comment)
        {
            return comment
                .Replace(" \r\n* ", " ")
                .Replace("\r\n* ", " ")
                .Replace(" \r\n * ", " ")
                .Replace("\r\n * ", " ");
        }

        public override string ToString()
        {
            return this.TestName;
        }
    }
}

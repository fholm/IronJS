using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Tests.Sputnik
{
    public class FailedTest
    {
        public string Name { get; set; }
        public string Path { get; set; }
        public string Assertion { get; set; }
        public string Exception { get; set; }
        public TestGroup TestGroup { get; set; }
        public bool Regression { get; set; }
    }
}

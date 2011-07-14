using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace IronJS.Tests.Sputnik
{
    public class TestDescriptor
    {
        public string Assertion { get; set; }

        public string Description { get; set; }

        public string Negative { get; set; }

        public string Path { get; set; }
    }
}

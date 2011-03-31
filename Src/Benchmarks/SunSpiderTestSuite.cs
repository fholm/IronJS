using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace Benchmarks
{
    public class SunSpiderTestSuite : TestSuite
    {
        public SunSpiderTestSuite(string basePath)
            : base(Path.Combine(basePath, "sunspider-0.9.1"))
        {
        }

        public override string SuiteName
        {
            get { return "SunSpider 0.9.1"; }
        }
    }
}

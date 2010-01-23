using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronJS.Tests
{
    [TestClass]
    public class StringRelationalTests
    {
        [TestMethod]
        public void TestStringRelations()
        {
            ScriptRunner.Run(
                @"
                    assert('foo' < 'foobar');
                    assert('foobar' > 'foo');
                    assert('foo' <= 'foo');
                    assert('foo' >= 'foo');
                    assert('fooa' < 'foob');
                    assert('foob' > 'fooa');
                "
            );
        }

    }
}

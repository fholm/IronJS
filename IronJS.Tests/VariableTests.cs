using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronJS.Tests
{
    [TestClass]
    public class VariableTests
    {
        [TestMethod]
        public void TestVariablesUnicodeNamesAreOk()
        {
            Assert.AreEqual(
                "foo",
                ScriptRunner.Run(
                    "åäö = 'foo'; emit(åäö);"
                )
            );
        }
    }
}

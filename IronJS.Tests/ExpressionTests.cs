using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronJS.Tests
{
    [TestClass]
    public class ExpressionTests
    {
        [TestMethod]
        public void TestParenExpr()
        {
            Assert.AreEqual(
                "25",
                ScriptRunner.Run(
                    "emit((1 + 4) * 5);"
                )
            );
        }

        [TestMethod]
        public void TestCommaExpr()
        {
            Assert.AreEqual(
                "3",
                ScriptRunner.Run(
                    "emit((1, 2, 3));"
                )
            );
        }
    }
}

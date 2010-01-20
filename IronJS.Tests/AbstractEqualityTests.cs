using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronJS.Tests
{
    [TestClass]
    public class AbstractEqualityTests
    {
        // 11.9.3, 2.
        [TestMethod]
        public void TestCompareUndefinedToUndefined()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(foo == undefined)" // foo == undefiend variable = undefined
                )
            );
        }

        // 11.9.3, 3.
        [TestMethod]
        public void TestCompareNullToNull()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(null == null)"
                )
            );
        }

        // 11.9.3, 3.
        [TestMethod]
        public void TestCompareNaNToNaN()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(+'' == NaN)" // +'' == ToNumber('') == NaN
                )
            );
        }

    }
}

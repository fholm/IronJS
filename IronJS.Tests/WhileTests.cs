using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronJS.Tests
{
    [TestClass]
    public class WhileTests
    {
        [TestMethod]
        public void TestWhile()
        {
            Assert.AreEqual(
                "01234",
                ScriptRunner.Run(
                    "i = 0; while(i < 5) { emit(i); ++i; } "
                )
            );
        }
    }
}

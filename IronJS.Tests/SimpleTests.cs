using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace IronJS.Tests
{
    [TestClass]
    public class SimpleTests
    {
        [TestMethod]
        public void TestStringConstant()
        {
            Assert.AreEqual(
                "foo",
                ScriptRunner.Run(
                    "emit(\"foo\")"
                )
            );
        }

        [TestMethod]
        public void TestNumberConstant()
        {
            Assert.AreEqual(
                "1",
                ScriptRunner.Run(
                    "emit((1).toString())"
                )
            );
        }

        [TestMethod]
        public void TestDecimalNumberConstant()
        {
            Assert.AreEqual(
                "3.14",
                ScriptRunner.Run(
                    "emit((3.14).toString())"
                )
            );
        }

        [TestMethod]
        public void TestBooleanTrueConstant()
        {
            Assert.AreEqual(
                "true",
                ScriptRunner.Run(
                    "emit(true.toString())"
                )
            );
        }

        [TestMethod]
        public void TestBooleanFalseConstant()
        {
            Assert.AreEqual(
                "false",
                ScriptRunner.Run(
                    "emit(false.toString())"
                )
            );
        }
    }
}
